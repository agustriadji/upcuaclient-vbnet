@echo off
echo Manually remove Console.WriteLine from these files:
echo.
echo 1. Forms\DetailRecord.vb - lines with commented Console.WriteLine
echo 2. Forms\FormNewRecord.vb - lines with commented Console.WriteLine  
echo 3. Forms\MainFormNew.vb - lines with commented Console.WriteLine
echo 4. Modules\Core\AnalyticsManager2.vb - debug Console.WriteLine
echo 5. Modules\Core\BackgroundWorker.vb - debug Console.WriteLine
echo 6. Modules\Core\SQLiteManager.vb - debug Console.WriteLine
echo.
echo Use Find/Replace in Visual Studio:
echo Find: Console.WriteLine
echo Replace: ' Console.WriteLine
echo.
echo This will comment out all Console.WriteLine statements
pause