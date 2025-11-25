# opc_server_with_tcp.py
import asyncio
import threading
import socket
from asyncua import ua, Server
import struct
import random
import time
from datetime import datetime

sensors_gauge = []
sensors_tire = []

def crc16_modbus(data):
    crc = 0xFFFF
    for byte in data:
        crc ^= byte
        for _ in range(8):
            if crc & 0x0001:
                crc = (crc >> 1) ^ 0xA001
            else:
                crc >>= 1
    return crc

def build_modbus_frame(values):
    slave_id = 0x01
    function_code = 0x03
    byte_count = len(values) * 4
    frame = bytearray([slave_id, function_code, byte_count])

    for val in values:
        float_bytes = struct.pack('<f', val)
        frame.extend(float_bytes)

    crc = crc16_modbus(frame)
    frame.extend(struct.pack('<H', crc))
    return frame

def send_dummy_modbus_data2():
    while True:
        try:
            dummy_values = [round(random.uniform(100, 200), 2) for _ in range(15)]
            frame = build_modbus_frame(dummy_values)

            with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as sock:
                sock.connect(('localhost', 5000))
                sock.sendall(frame)
                print(f"🚀 Sent dummy Modbus frame: {dummy_values}")
        except Exception as e:
            print(f"❌ Dummy sender error: {e}")
        time.sleep(5)

def parse_modbus_frame(data):
    if len(data) < 5:
        return None

    slave_id = data[0]
    function_code = data[1]

    if function_code == 0x03:
        byte_count = data[2]
        if len(data) < 5 + byte_count:
            return None

        register_data = data[3:3+byte_count]
        crc_received = struct.unpack('<H', data[3+byte_count:5+byte_count])[0]
        crc_calculated = crc16_modbus(data[:3+byte_count])
        if crc_received != crc_calculated:
            print(f"❌ CRC Error: received {crc_received:04X}, calculated {crc_calculated:04X}")
            return None

        values = []
        for i in range(0, len(register_data), 4):
            if i + 4 <= len(register_data):
                float_bytes = register_data[i:i+4]
                value = struct.unpack('<f', float_bytes)[0]
                values.append(value)

        return values

    return None

def run_tcp_server():
    server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    server_socket.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
    server_socket.bind(('0.0.0.0', 5000))
    server_socket.listen(1)
    print("🔌 TCP Server listening on port 5000")

    while True:
        try:
            client_socket, addr = server_socket.accept()
            print(f"📡 Client connected: {addr}")

            while True:
                data = client_socket.recv(1024)
                if not data:
                    break

                print(f"📦 Received {len(data)} bytes: {data.hex()}")

                values = parse_modbus_frame(data)
                if values:
                    print(f"✅ Parsed {len(values)} values: {values}")

                    loop = asyncio.new_event_loop()
                    asyncio.set_event_loop(loop)

                    for i, value in enumerate(values[:15]):
                        if i < len(sensors_gauge):
                            loop.run_until_complete(
                                sensors_gauge[i].write_value(ua.Variant(value, ua.VariantType.Float))
                            )
                        if i < len(sensors_tire):
                            loop.run_until_complete(
                                sensors_tire[i].write_value(ua.Variant(value, ua.VariantType.Float))
                            )

                    loop.close()
                    print(f"[{datetime.now()}] ✅ Updated sensors with Modbus data")

        except Exception as e:
            print(f"❌ TCP Server error: {e}")
        finally:
            try:
                client_socket.close()
            except:
                pass

async def run_opcua():
    server = Server()
    await server.init()
    server.set_endpoint("opc.tcp://localhost:4840")
    server.set_server_name("PythonOPC.KepwareClone")

    uri = "http://python-opc-simulation.local"
    idx = await server.register_namespace(uri)

    objects = server.nodes.objects
    pressure_gauge = await objects.add_object(idx, "PressureGauge")
    pressure_tire = await objects.add_object(idx, "PressureTire")

    for i in range(1, 16):
        sensors_gauge.append(await pressure_gauge.add_variable(
            idx, f"Sensor {i}", ua.Variant(0.0, ua.VariantType.Float), ua.VariantType.Float))
        sensors_tire.append(await pressure_tire.add_variable(
            idx, f"Sensor {i}", ua.Variant(0.0, ua.VariantType.Float), ua.VariantType.Float))

    for sensor in sensors_gauge + sensors_tire:
        await sensor.set_writable()

    print("✅ OPC UA Server ready at: opc.tcp://localhost:4840/freeopcua/server/")
    print("🌐 Flask API ready at: http://localhost:5000/sensors")

    async with server:
        while True:
            for i in range(15):
                val1 = ua.Variant(round(200 + random.uniform(-5, 5), 2), ua.VariantType.Float)
                val2 = ua.Variant(round(150 + random.uniform(-5, 5), 2), ua.VariantType.Float)
                await sensors_gauge[i].write_value(val1)
                await sensors_tire[i].write_value(val2)
            print(f"[{datetime.now()}] ✅ Updated PressureGauge & PressureTire sensors")
            await asyncio.sleep(5)
            
def send_dummy_modbus_data():
    total_sensors = 30
    frame = bytearray(3 + total_sensors * 4 + 2)
    frame[0] = 0x01  # Device ID
    frame[1] = 0x03  # Function code
    frame[2] = total_sensors * 4  # Byte count

    for i in range(total_sensors):
        is_inner = i < 15
        max_pressure = 600.0 if is_inner else 800.0

        current = random.randint(400, 2000) / 100.0  # 4.00–20.00 mA
        pressure = (current - 4.0) * (max_pressure / 16.0)
        pressure = max(0.0, min(pressure, max_pressure))

        float_bytes = struct.pack('<f', pressure)
        frame[3 + i * 4 : 3 + (i + 1) * 4] = float_bytes

        print(f"Sensor[{i+1:02}] = {pressure:.2f} kPa ({current:.2f} mA)")

    crc = crc16_modbus(frame[:3 + total_sensors * 4])
    frame[-2:] = struct.pack('<H', crc)

    try:
        with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as sock:
            sock.connect(('localhost', 5000))
            sock.sendall(frame)
            print(f"🚀 Sent Modbus frame ({len(frame)} bytes)")
    except Exception as e:
        print(f"❌ Dummy sender error: {e}")

if __name__ == "__main__":
    tcp_thread = threading.Thread(target=run_tcp_server, daemon=True)
    # dummy_thread = threading.Thread(target=send_dummy_modbus_data, daemon=True)
    dummy_thread = threading.Thread(target=lambda: [send_dummy_modbus_data() or time.sleep(5) for _ in iter(int, 1)], daemon=True)

    tcp_thread.start()
    dummy_thread.start()

    asyncio.run(run_opcua())