/*
 * 
 * Author:      Kieron Higgs, Arie Kraayenbrink
 * Date:        12-09-2018
 * Project:     EMS1
 * File:        Support - FileIO.cs
 * Description: Contains the code related to File IO operations in the EMS application.
 * 
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EMS_2;

namespace Support
{
	/// <summary>
	/// The FileIO class contains all of the methods used for reading and writing from the database.
	/// 
	/// Fault Detection:
	/// - Reading and writing procedures are embedded in try-catch blocks to prevent system failure upon error
	/// - Logging fully implemented for IO procedures to document area of failure in case of incidents
	/// - Requires some more work; once the method is written for saving the database, 'emergency' backup processes may be implemented
	/// Relationships:
	/// - Contains no knowledge of the other classes, as per requirements. The role of this class is to facilitate IO operations for the other classes,
	/// but not to carry out actions belonging to the domain of the Scheduling or Demographics modules.
	/// </summary>
	public static class FileIO
	{
		/**
        * \brief Checks whether the databases exist for subsequent File IO operations.
        * \return bool
        * \author Kieron Higgs
        */
		public static bool CheckForDB()
		{
			// Use CheckForFile() to see if the database files exist, and create it if not:
			if (CheckForFile(Constants.DBFiles[1]) == false || CheckForFile(Constants.DBFiles[2]) == false)
			{
				return false;
			}
			return true;
		}

		/**
        * \brief Checks whether a file exists based on the given filename or filepath. If it does not exist,
        * the method will create it.
        * \param string filePath
        * \return bool
        * \author Kieron Higgs
        */
		public static bool CheckForFile(string filePath)
		{
			if (!File.Exists(filePath))
			{
				try
				{
					File.WriteAllText(filePath, "");

					Logging.Write("Created a new file.");
				}
				// In case of error, catch the exception for use by the Logging class:
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());

					Logging.Write("Error occured while opening file.");
					return false;
				}
			}
			return true;
		}


        /**
        * \brief Checks whether the given filename string corresponds to a file which exists.
        * \return bool
        * \author Arie Kraayenbrink
        */
        public static bool CheckFileExists(string fileName)
		{
			bool exits = false;

			exits = File.Exists(fileName);

			return exits;
		}

        /**
        * \brief Allows a file to be renamed.
        * \param path - the given filepath
        * \param fileName - the name of the desired file
        * \param extension - the desired file's extension
        * \return bool - true if successful
        * \author Arie Kraayenbrink
        */
        public static bool RenameFile(string path, string fileName, string extension)
		{
			bool success = false;

			success = CheckFileExists(path + @"\" + fileName + extension);
			if (success)
			{
				string[] files = Directory.GetFiles(path);
				int fileCount = files.Count(File => { return File.Contains(fileName); }); //Credit: https://www.codeproject.com/Questions/212217/increment-filename-if-file-exists-using-csharp

				string newFileName = (fileCount == 0) ? (fileName + extension) : String.Format("{0}({1}).bak", fileName, fileCount + 1);

				try
				{
					//rename the file
					File.Move(path + @"\" + fileName + extension, path + @"\" + newFileName);
				}
				catch (Exception e)
				{
					Logging.Write("Error occured while renaming file. " + e.Message);
					success = false;
				}
				finally
				{

				}
			}

			return success;
		}



        ///-------------------------------------------------------------------------------------------------
        /// \fn public static bool ReadLines(string fileName, out string[] buff)
        ///
        /// \brief  Reads the lines
        ///
        /// \author Arie
        /// \date   2019-04-22
        ///
        /// \param          fileName    Filename of the file.
        /// \param [out]    buff        The buffer.
        ///
        /// \returns    True if it succeeds, false if it fails.
        ///-------------------------------------------------------------------------------------------------
        public static bool ReadLines(string fileName, out string[] buff)
        {
            bool success = false;
            StreamReader sr = null;
            buff = null;

            success = CheckFileExists(fileName);
            if (success)
            {
                try
                {
                    sr = new StreamReader(fileName);

                    buff = File.ReadAllLines(fileName);
                }
                catch (Exception e)
                {
                    Logging.Write("Error occured while opening file. " + e.Message);
                    success = false;
                }
                finally
                {
                    if (sr != null)
                    {
                        sr.Close();
                    }
                }
            }

            return success;
        }



        ///-------------------------------------------------------------------------------------------------
        /// \fn public static int CountLines(string fileName)
        ///
        /// \brief  Count lines
        ///
        /// \author Arie
        /// \date   2019-04-22
        ///
        /// \param  fileName    Filename of the file.
        ///
        /// \returns    The total number of lines.
        ///-------------------------------------------------------------------------------------------------
        public static int CountLines(string fileName)
        {
            int lineCount = 0;
            string[] buff = null;

            if (CheckFileExists(fileName))
            {
                try
                {
                    // Read the file
                    ReadLines(fileName, out buff);

                    // Count number of lines
                    //lineCount = buff.Count(); // Total lines

                    foreach (var line in buff)
                    {
                        // Only none commented lines are counted. 
                        if (!line.StartsWith("--") && line != "")
                        {
                            lineCount++;
                        }
                    }
                }
                catch (Exception e)
                {
                    Logging.Write("Error occurred while opening file. " + e.Message);
                    lineCount = -1;
                }
            }
            else
            {
                Logging.Write("File [" + fileName + "] was not found.");
                lineCount = -1;
            }

            return lineCount;
        }



        /**
        * \brief Reads the contents of a file into a string which is returned via an out parameter.
        * \param string fileName - the given file name
        * \param out string buff - the buffer which will contain the resulting data
        * \return bool - true if successful
        * \author Arie Kraayenbrink
        */
        public static bool ReadFile(string fileName, out string buff)
		{
			bool success = false;
			StreamReader sr = null;
			buff = null;

			success = CheckFileExists(fileName);
			if (success)
			{
				try
				{
					sr = new StreamReader(fileName);

					//buff = sr.ReadLine();
					buff = sr.ReadToEnd().ToString();
				}
				catch (Exception e)
				{
					Logging.Write("Error occured while opening file. " + e.Message);
					success = false;
				}
				finally
				{
					if (sr != null)
					{
						sr.Close();
					}
				}
			}

			return success;
		}

        /**
        * \brief Writes content to a file. The content and filename are passed to the method.
        * \param string fileName - the given file name
        * \param string buff - the buffer which contains the data to be written
        * \return bool - true if successful
        * \author Arie Kraayenbrink
        */
        public static bool WriteFile(string fileName, string buff)
		{
			bool success = false;
			StreamWriter sr = null;
            FileStream fs = null;


            success = CheckFileExists(fileName);
			if (!success)
			{
				try
				{
					fs = File.Create(fileName);
                    
                    success = true;
				}
				catch (Exception e)
				{
					Logging.Write("Error occured while creating file. " + e.Message);
					success = false;
				}
				finally
				{
                    if (fs != null)
                    {
                        fs.Close();
                    }
                }
			}

			if (success)
			{
				try
				{
					sr = new StreamWriter(fileName);
					sr.WriteLine(buff);

				}
				catch
				{
					Logging.Write("Error occured while writing to file.");
					success = false;
				}
				finally
				{
					if (sr != null)
					{
						sr.Close();
					}
				}
			}
            
			return success;
		}

        /**
        * \brief Lists the files in a directory.
        * \author Arie Kraayenbrink
        */
        public static void ListFiles()
        {
            //list files in db folder
            int i = 1;
            foreach (var path in Directory.GetFiles(Constants.dbDirectory))
            {
                Console.Write("(" + i + ")" + path); // full path with selection number
                Console.WriteLine(Path.GetFileName(path)); // file name
            }
        }

        /**
        * \brief Creates an array list containing the names of the text files in a given directory.
        * \param string dir - the given directory
        * \return ArrayList - the resulting array list containing the file names
        * \author Arie Kraayenbrink
        */
        public static ArrayList ListFiles(string dir)
        {
            //count number text of files
            int fileCount = Directory.GetFiles(dir, "*.txt", SearchOption.TopDirectoryOnly).Length;

            //list files in db folder
            string[][] data = new string[fileCount][];
            int i = 0;
            foreach (var path in Directory.GetFiles(dir))
            {
                string filePath = path;
                if (path.EndsWith(".txt"))
                {                    
                    string fileName = Path.GetFileName(path);

                    data[i] = new string[] { i.ToString(), fileName };
                    i++;
                }
            }

            ArrayList arr = new ArrayList(data);
            return arr;
        }

        /**
        * \brief Creates two lists containing the filepaths and filenames of the files in the database directory.
        * \param out List<string> filePath - the given filepath
        * \param out List<string> fileName - the given file name
        * \author Arie Kraayenbrink
        */
        public static void ListFiles(out List<string> filePath, out List<string> fileName)
        {
            filePath = null;
            fileName = null;
            filePath.Clear();
            fileName.Clear();

            //list files in db folder            
            foreach (var path in Directory.GetFiles(Constants.dbDirectory))
            {                
                filePath.Add(path); // full path
                fileName.Add(Path.GetFileName(path)); // file name               
            }
        }

        /**
        * \brief Creates a FileStream object and opens a file for reading.
        * \param string fileName - the given file name
        * \param out bool valid - indicates the existence of the file
        * \return FileStream - the filestream connected to the desired file
        * \author Arie Kraayenbrink
        */
        public static FileStream OpenFile(string fileName, out bool valid)
		{
			FileStream f = null;
			valid = false;

			valid = CheckFileExists(fileName);

			if (valid)
			{
				try
				{
					f = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
				}
				catch
				{
					Logging.Write("Error occured while opening file.");

					valid = false;
				}
				finally
				{
					f.Close();
				}

			}

			return f;
		}

        /**
        * \brief Closes a file with the given applicable FileStream object.
        * \param FileStream f - the given filestream 
        * \return bool - true if successful
        * \author Arie Kraayenbrink
        */
        public static bool CloseFile(FileStream f)
		{
			bool valid = false;

			try
			{
				f.Close();
			}
			catch
			{
				Logging.Write("Error occured while closing file.");

				valid = false;
			}
			finally
			{

			}

			return valid;
		}


		/**
        * \brief Provides a StreamReader object for the desired text file for getting records from the database.
        * \param string filePath
        * \return StreamReader
        * \author Kieron Higgs
        */
		public static StreamReader OpenDB(string filePath)
		{
			return new StreamReader(filePath);
		}

		/**
        * \brief Reads the next line of the database connected to the StreamReader.
        * \param ref StreamReader DBReader
        * \return string
        * \author Kieron Higgs
        */
		public static string GetNextRecord(ref StreamReader DBReader)
		{
			string nextRecord;
			try
			{
				nextRecord = DBReader.ReadLine();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());

				Logging.Write("Error occured while reading from file.");
				return "error";
			}
			return nextRecord;
		}

		/**
        * \brief Closes the StreamReader (and thus "closes" the associated databsee file).
        * \param StreamReader DBReader
        * \author Kieron Higgs
        */
		public static void CloseDB(StreamReader DBReader)
		{
			DBReader.Close();
		}

		/**
        * \brief Adds a new record to the database indicated by the given filepath.
        * \param string newRecord
        * \param string filePath
        * \author Kieron Higgs
        */
		public static bool AddRecord(string newRecord, string filePath)
		{
			try
			{
				StreamWriter DBWriter = new StreamWriter(filePath);
				DBWriter.WriteLine(newRecord);
                DBWriter.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());

				Logging.Write("Error occured while writing to file.");
				return false;
			}
			return true;
		}


        /**
        * \brief Clears a text database for rewriting.
        * \param string filePath - the filepath to the database
        * \author Kieron Higgs
        */
        public static bool ClearDB(string filePath)
		{
			try
			{
				File.WriteAllText(filePath, String.Empty);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());

				Logging.Write("Error occured while writing to file.");
				return false;
			}
			return true;
		}
    }
}