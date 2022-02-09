//Load the CsvHelper in Tools/NuGet Package Manager
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using System.Globalization;
using System.Collections;

int MAX_JOB_ROWS = 75000;
int MAX_PAYS = 300;
//bool RUN_IF_ROW_ERRORS = true;




//Ask user for the directory of files
//Check if the Directoy exists for them???
Console.Write("Where is the main directory for the .csv files? ");
var directorySourcePath = @Console.ReadLine();

//For testing, you can loop through all of the files if you want
while (true)
{
    //Ask user for the file name
    Console.Write("What is the name of the .csv file you would like to parse? ");
    var fileName = @Console.ReadLine();

    //Put the directory and file name together for one path
    var fullFileNameSourceLocation = directorySourcePath + @"\" + fileName; //could use combine, need error handling for null though

    while (true)
    {
        //Check if the file exists, if it does print and change state
        if (File.Exists(fullFileNameSourceLocation))
        {
            Console.WriteLine();
            Console.WriteLine("File Exists!\n");
        }
        else
        {
            Console.WriteLine("File does not exist in the following path: " + fullFileNameSourceLocation + "\n");
            break;
        }

        //Check if the file is empty or not, if it does print and change state
        if (new FileInfo(fullFileNameSourceLocation).Length != 0)
        {
            Console.WriteLine("File is not Empty!\n");
        }
        else
        {
            Console.WriteLine("File is empty at: " + fullFileNameSourceLocation + "\n");
            break;
        }

        //Copy File to working directory if it does exist and is not empty
        // Create a new target folder, if necessary.
        var directoryTargetPath = @"C:\Users\dills\Desktop\ParseFiles\WorkingFolder";
        if (!System.IO.Directory.Exists(directoryTargetPath))
        {
            System.IO.Directory.CreateDirectory(directoryTargetPath);
        }

        var fullFileNameTargetLocation = directoryTargetPath + @"\" + fileName;
        System.IO.File.Copy(fullFileNameSourceLocation, fullFileNameTargetLocation, true);


        //Parse the files
        //Actually open the file and do some work if the file exists
        using (var streamReader = new StreamReader(fullFileNameTargetLocation))
        {
            using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
            {
                //Check to see if the Header Names match the expected
                try
                {
                    //Map the column names if order is correct and create List to work with
                    csvReader.Context.RegisterClassMap<JobsClassMap>();
                    var jobList = csvReader.GetRecords<Jobs>().ToList();

                    //Column names should match and be of correct length before map or break
                    var columnName = csvReader.HeaderRecord;
                    string[] headerMatchTest = { "RPDJobID", "PaySeq#", "BatchType", "TotalPays", "" +
                            "PkgDisp", "ProcessedTime", "MFTSystemID", "RejectReason" };
                    bool headersMatch = columnName.SequenceEqual(headerMatchTest);

                    ///*
                    //Throw exeption/error/or Ignore if we have a column list that is not a length of 8
                    if (columnName.Length != 8 || !headersMatch)
                    {
                        Console.WriteLine("The column quantity or names do not match expected Error 2!\n");
                        break;
                    }
                    //*/


                    //Print the column names
                    Console.WriteLine("The Column Names Are:");
                    foreach (var name in columnName)
                    {
                        Console.WriteLine(name);
                    }

                    Console.WriteLine("\nTotal List Count: " + jobList.Count);
                    Console.WriteLine("Total Column Count: " + jobList[0].GetType().GetProperties().Length + "\n");

                    //Check for the number of rows to continue
                    if (jobList.Count > MAX_JOB_ROWS)
                    {
                        Console.WriteLine("Too many jobs in file! Error 3\n");
                        break;
                    }

                    //Check for the Job Name to be the same, is this just for the barcode or can we delete


                    //Check to make sure we are running E's or F's

                    //

                    //Check to make sure duplicate Pay Seq#s are not detected, log row and sequence number
                    //Move pay sequence into list by themselves??? There has to be a better way
                    //Pay Sequence should always match row
                    var paySequenceCheck = new ArrayList();
                    var payRowsWithDuplicatePaySequences = new ArrayList();
                    for (int i = 0; i < jobList.Count; i++)
                    {
                        if (!paySequenceCheck.Contains(jobList[i].PaySequence) && jobList[i].PaySequence == i + 1)
                        {
                            paySequenceCheck.Add(jobList[i].PaySequence);
                        }
                        else
                        {
                            payRowsWithDuplicatePaySequences.Add(i + 1);
                        }
                    }

                    if (payRowsWithDuplicatePaySequences.Count != 0)
                    {
                        Console.WriteLine("We have duplicate pay sequences, or sequence doesn't match row. Error 4");
                        Console.Write("They are located here: ");
                        foreach (Object pays in payRowsWithDuplicatePaySequences)
                        {
                            Console.Write(pays + " ");
                        }
                        Console.WriteLine("\n");
                        break;
                    }


                    int numberOfRows = 0;

                    while (true)
                    {
                        try
                        {
                            numberOfRows = AskForNumberInRange("How many rows do you want to see (Between 1 and List Count):", 1, jobList.Count);
                            break;
                        }
                        catch (Exception)
                        {

                            Console.WriteLine("That was the bad number! Try again. \n");
                        }
                    }


                    //Loop through all of the stuff and print (we can do a check for each data member as well)
                    for (int i = 0; i < numberOfRows; i++)
                    {
                        Console.WriteLine("\nRow #" + (i + 1) + " contains the following:");
                        int j = 0;

                        //put into method
                        ArrayList tempList = new ArrayList();
                        tempList.Add(jobList[i].RPDJobID);
                        tempList.Add(jobList[i].PaySequence);
                        tempList.Add(jobList[i].BatchType);
                        tempList.Add(jobList[i].TotalPays);
                        tempList.Add(jobList[i].PackageDisp);
                        tempList.Add(jobList[i].ProcessedTime);
                        tempList.Add(jobList[i].MFTSystemID);
                        tempList.Add(jobList[i].RejectReason);


                        foreach (var name in columnName)
                        {
                            Console.WriteLine(name + ": " + tempList[j]);
                            j++;
                        }
                    }


                    //PRITER STUFF ADDED
                    //Check if the file exists, if it does print and change state
                    if (File.Exists(fullFileNameSourceLocationZPL))
                    {
                        //Console.WriteLine();
                        //Console.WriteLine("File Exists!\n");
                        //Send status bits to PLC
                    }
                    else
                    {
                        Console.WriteLine("File does not exist in the following path: " + fullFileNameSourceLocationZPL + "\n");
                        break;
                    }

                    //Check if the file is empty or not, if it does print and change state
                    if (new FileInfo(fullFileNameSourceLocationZPL).Length != 0)
                    {
                        //Console.WriteLine("File is not Empty!\n");
                        //Send status bits to PLC
                    }
                    else
                    {
                        Console.WriteLine("File is empty at: " + fullFileNameSourceLocationZPL + "\n");
                        break;
                    }


                    //Read the file as one string.
                    var stringZPL = System.IO.File.ReadAllText(fullFileNameSourceLocationZPL);


                    // Display the file contents to the console. Variable text is a string.
                    //System.Console.WriteLine("Contents of ZPL = {0}", stringZPL);

                    FileInfo info = new FileInfo(fullFileNameSourceLocationZPL);

                    //Make sure that we don't have a crazy giant 2GB file
                    try
                    {
                        int myInfo = checked(Convert.ToInt32(info.Length));
                        //Console.WriteLine(myInfo);
                        SendZplOverTcp("127.0.0.1", stringZPL, myInfo);
                    }
                    catch (ConnectionException e)
                    {
                        // Handle communications error here.
                        //Console.WriteLine(e.ToString());
                        Console.WriteLine("File too large be sent to the printer: " + fullFileNameSourceLocationZPL + "\n");
                        //Send status bits to PLC
                        break;
                    }




                    Console.WriteLine("\nPress any key to enter a new file name. ");
                    Console.ReadKey();
                    Console.WriteLine();
                    break;

                }

                catch (Exception)
                {
                    Console.WriteLine("The column quantity or names do not match expected! Error 1\n");
                    break;
                }




                //Header Name Check (PLC Configurable???)

                //Column Check (PLC Configurable???)

                //Row Check 1-75000 (PLC Configurable???)


                //sw.Stop();


            }
        }
    }

}

int AskForNumber(string text)
{
    Console.Write(text + " ");
    int number = Convert.ToInt32(Console.ReadLine());
    return number;
}

int AskForNumberInRange(string text, int min, int max)
{
    while (true)
    {
        int number = AskForNumber(text);
        if (number >= min && number <= max)
            return number;
    }
}

public class JobsClassMap : ClassMap<Jobs>
{
    public JobsClassMap()
    {
        Map(m => m.RPDJobID).Name("RPDJobID");
        Map(m => m.PaySequence).Name("PaySeq#");
        Map(m => m.BatchType).Name("BatchType");
        Map(m => m.TotalPays).Name("TotalPays");
        Map(m => m.PackageDisp).Name("PkgDisp");
        Map(m => m.ProcessedTime).Name("ProcessedTime");
        Map(m => m.MFTSystemID).Name("MFTSystemID");
        Map(m => m.RejectReason).Name("RejectReason");
    }
}


public class Jobs
{
    public int RPDJobID { get; set; }
    public int PaySequence { get; set; }
    public string BatchType { get; set; }
    public int TotalPays { get; set; }
    public string PackageDisp { get; set; }
    public string ProcessedTime { get; set; }
    public int MFTSystemID { get; set; }
    public string RejectReason { get; set; }
}

