using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace SIT323_Assignment1_v2
{
    public partial class CrozzleGame : Form
    {
        public CrozzleGame()
        {
            InitializeComponent();
        }

        #region Variables
        //Variables
        string txtFileName;
        string txtFileNameSafe;
        string csvFileName;
        string csvFileNameSafe;
        int beginningOfList = 0;
        int listStartOfNames = 4;
        int score = 0;
        bool intersectEM = false;
        bool intersectHEx = false;
        #endregion
        #region GUI objects
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        #endregion



        #region Buttons that select the TxT and CSV files
        //Create OpenFileDirectory
        OpenFileDialog ofd = new OpenFileDialog();

        private void button1_Click(object sender, EventArgs e)
        {
            ofd.Filter = "TXT|*.txt;";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = ofd.SafeFileName;
                txtFileNameSafe = ofd.SafeFileName;
                txtFileName = ofd.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ofd.Filter = "CSV|*.csv";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = ofd.SafeFileName;
                csvFileNameSafe = ofd.SafeFileName;
                csvFileName = ofd.FileName;
            }
        }
        #endregion

        #region Button that loads the files to the GUI
        private void button3_Click(object sender, EventArgs e)
        {
            #region FileStrem Open for log file
            FileStream fin;
            StreamReader fstr_in;
            StreamWriter fstr_out;
            string str;
            #endregion


            dataGridView1.Rows.Clear();
            bool errorMessage = false;
            List<string> listA = new List<string>();
            List<string> listB = new List<string>();
            List<string> tempCrozzleLetters = new List<string>();
            List<string> CrozzleLetters = new List<string>();

            //Read CSV File
            StreamReader reader = new StreamReader(File.OpenRead(csvFileName));

            //Add contents to list
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                listA = line.Split(',').ToList();
            }

            listB = listA.GetRange(beginningOfList, listStartOfNames);
            listA.RemoveRange(beginningOfList, listStartOfNames);

            /*
             * Create reader, named reader2, to read the contents of the crozzle text file.
             * Then store the words of the file in a list called CrozzleWords.
             */
            StreamReader reader2 = new StreamReader(File.OpenRead(txtFileName));
            while (!reader2.EndOfStream)
            {
                char ch = (char)reader2.Read();
                if (ch != null)
                {
                    string letter = ch.ToString();
                    CrozzleLetters.Add(letter);
                }
            }

            #region Regular Expressions
            Regex regexFilter = new Regex(@"^[0-9!@#$%^&*()~_+-={};:',<>./?\|]+$");
            Regex regexFilterDigits = new Regex(@"\d");
            Regex regexFilterWords = new Regex(@"(EASY|MEDIUM|HARD|EXTREME)");
            foreach (string item in CrozzleLetters)
            {
                if (regexFilter.IsMatch(item))
                {
                    MessageBox.Show("Invalid text file.");
                    errorMessage = true;
                    fstr_out = new StreamWriter(@"../../LogFile.txt", true);
                    fstr_out.WriteLine(txtFileNameSafe + " - Error: Invalid text file");
                    fstr_out.Close();
                    break;
                }
            }

            foreach (string item in listA)
            {
                if (regexFilter.IsMatch(item))
                {
                    MessageBox.Show("Invalid CSV file - Word list contains a number or unrecognised symbol.");
                    errorMessage = true;
                    fstr_out = new StreamWriter(@"../../LogFile.txt", true);
                    fstr_out.WriteLine(csvFileNameSafe + " - Error: Invalid CSV file - Word list contains a number or unrecognised symbol.");
                    fstr_out.Close();
                    break;
                }
            }

            foreach (string item in listB.GetRange(0, 3))
            {
                if (!regexFilterDigits.IsMatch(item))
                {
                    MessageBox.Show("Invalid CSV file - The CSV file does not contain valid numbers for dimensions or for number of words.");
                    errorMessage = true;
                    fstr_out = new StreamWriter(@"../../LogFile.txt", true);
                    fstr_out.WriteLine(csvFileNameSafe + " - Error: Invalid CSV file - The CSV file does not contain valid numbers for dimensions or for number of words.");
                    fstr_out.Close();
                    break;
                }
            }

            foreach (string item in listB.Skip(3))
            {
                if (!regexFilterWords.IsMatch(item))
                {

                    MessageBox.Show("Invalid CSV file - Invalid difficulty setting.");
                    errorMessage = true;
                    fstr_out = new StreamWriter(@"../../LogFile.txt", true);
                    fstr_out.WriteLine(csvFileNameSafe + " - Error: Invalid CSV file - Invalid difficulty setting.");
                    fstr_out.Close();
                    break;
                }
            }
            #endregion

            if (errorMessage != true)
            {
                /*
                 * \n and \r represent the end and beginning of a line in a txt file.
                 * These are read along with all text taken from the txt file and placed in the list.
                 * However these symbols increase the size of the list which prevents the array from accepting all elements of the crozzle
                 * Removing these string are necessary to create an identical multi dimensional array with the same layout as the txt file.
                 */
                string remove2 = "\n";
                string remove1 = "\r";
                CrozzleLetters.RemoveAll(u => u.Contains(remove1));
                CrozzleLetters.RemoveAll(u => u.Contains(remove2));


                /*
                 *This segment of code allows the multidimensional array to be formed with the correct dimensions for the crozzle
                 *The dimensions are taken from listB which contains the number of words to be used, the number of rows and columns(dimensions) and the difficulty.
                 */


                int arrayRow = Convert.ToInt32(listB[1]);
                int arrayColumn = Convert.ToInt32(listB[2]);
                //Create crozzle array for recreating crozzle from txt file
                string[,] CrozzleArray = new string[arrayRow, arrayColumn];
                List<string> CrozzleWords = new List<string>();
                int k = 0;

                for (int i = 0; i < CrozzleArray.GetLength(0); i++)
                {
                    for (int j = 0; j < CrozzleArray.GetLength(1); j++)
                    {
                        while (k < CrozzleLetters.Count)
                        {
                            string listLetter;
                            listLetter = CrozzleLetters[k];
                            CrozzleArray[i, j] = listLetter;
                            k++;
                            break;
                        }
                    }
                }

                if (CrozzleArray.GetLength(0) < 4 || CrozzleArray.GetLength(0) > 400)
                {
                    MessageBox.Show("Invalid Crozzle - Crozzles width is not valid");
                    errorMessage = true;
                    fstr_out = new StreamWriter(@"../../LogFile.txt", true);
                    fstr_out.WriteLine(txtFileNameSafe + "/" + csvFileNameSafe + " - Error: Invalid Crozzle - Crozzles width is not valid");
                    fstr_out.Close();
                }

                if (CrozzleArray.GetLength(1) < 4 || CrozzleArray.GetLength(0) > 400)
                {
                    MessageBox.Show("Invalid Crozzle - Crozzles length is not valid");
                    errorMessage = true;
                    fstr_out = new StreamWriter(@"../../LogFile.txt", true);
                    fstr_out.WriteLine(txtFileNameSafe + "/" + csvFileNameSafe + " - Error: Invalid Crozzle - Crozzles length is not valid");
                    fstr_out.Close();
                }

                if (errorMessage != true)
                {
                    for (int i = 0; i < CrozzleArray.GetLength(0); i++)
                    {
                        for (int j = 0; j < CrozzleArray.GetLength(1); j++)
                        {
                            if (CrozzleArray.GetValue(i, j) == null)
                            {
                                MessageBox.Show("Invalid text file - Crozzle uses incorrect dimensions compared to those in the CSV file");
                                errorMessage = true;
                                fstr_out = new StreamWriter(@"../../LogFile.txt", true);
                                fstr_out.WriteLine(txtFileNameSafe + " - Error: Invalid text file - Crozzle uses incorrect dimensions compared to those in the CSV file");
                                fstr_out.Close();
                                break;
                            }
                        }
                    }

                    string letterCurrent;
                    string letterRight;
                    string letterLeft;
                    string letterBelow;
                    string letterAbove;
                    int letterBelowCount = 0;
                    int letterAboveCount = 0;
                    int letterRightCount = 0;
                    int letterLeftCount = 0;
                    string formedWord = "";

                    List<string> ValidWords = new List<string>();

                    //Crozzle Validation
                    #region Horizontal word validation
                    //n = i
                    //m = j
                    for (int i = 0; i < CrozzleArray.GetLength(0); i++)
                    {
                        for (int j = 0; j < CrozzleArray.GetLength(1); j++)
                        {
                            
                            if ((string)CrozzleArray.GetValue(i, j) != " ")
                            {
                                #region Try Catches
                                letterCurrent = (string)CrozzleArray.GetValue(i, j);
                                try
                                {
                                    letterRight = (string)CrozzleArray.GetValue(i, j + 1);
                                }
                                catch
                                {
                                    letterRight = null;
                                }

                                try
                                {
                                    letterLeft = (string)CrozzleArray.GetValue(i, j - 1);
                                }
                                catch
                                {
                                    letterLeft = null;
                                }

                                try
                                {
                                    letterBelow = (string)CrozzleArray.GetValue(i + 1, j);
                                }
                                catch
                                {
                                    letterBelow = null;
                                }
                                #endregion

                                if (letterRight != null)
                                {
                                    for (int m = j; m < CrozzleArray.GetLength(1); m++)
                                    {
                                        letterCurrent = (string)CrozzleArray.GetValue(i, m);

                                        #region Try Catches
                                        try
                                        {
                                            letterRight = (string)CrozzleArray.GetValue(i, m + 1);
                                        }
                                        catch
                                        {
                                            letterRight = null;
                                        }

                                        try
                                        {
                                            letterLeft = (string)CrozzleArray.GetValue(i, m - 1);
                                        }
                                        catch
                                        {
                                            letterLeft = null;
                                        }

                                        try
                                        {
                                            letterBelow = (string)CrozzleArray.GetValue(i + 1, m);
                                            if (!String.IsNullOrWhiteSpace(letterBelow))
                                            {
                                                letterBelowCount++;
                                            }
                                        }
                                        catch
                                        {
                                            letterBelow = null;
                                        }

                                        try
                                        {
                                            letterAbove = (string)CrozzleArray.GetValue(i - 1, m);
                                            if (!String.IsNullOrWhiteSpace(letterAbove))
                                            {
                                                letterAboveCount++;
                                            }
                                        }
                                        catch
                                        {
                                            letterAbove = null;
                                        }
                                        #endregion

                                        //Checks if word is passing through more than two other words
                                        if (errorMessage != true)
                                        {
                                            if (letterBelowCount >= 3 || letterAboveCount >= 3)
                                            {
                                                intersectEM = true;
                                            }
                                        }

                                        /*if (errorMessage != true)
                                        {
                                            if (letterBelowCount < 1 || letterAboveCount < 1)
                                            {
                                                intersectHEx = true;
                                            }
                                        }*/



                                        if (!String.IsNullOrWhiteSpace(letterRight))
                                        {
                                            formedWord = formedWord + letterCurrent;
                                        }
                                        else if (!String.IsNullOrWhiteSpace(letterLeft))
                                        {
                                            formedWord = formedWord + letterCurrent;
                                            j = m;
                                            break;
                                        }
                                        else
                                        {
                                            
                                            break;
                                        }
                                    }
                                    if (listA.Contains(formedWord) == true && errorMessage != true)
                                    {
                                        if (ValidWords.Contains(formedWord) == true)
                                        {
                                            MessageBox.Show("Invalid Crozzle - Words have been used more than once.");
                                            errorMessage = true;
                                            fstr_out = new StreamWriter(@"../../LogFile.txt", true);
                                            fstr_out.WriteLine(txtFileNameSafe + "/" + csvFileNameSafe + " - Error: Invalid Crozzle - Words have been used more than once.");
                                            fstr_out.Close();
                                            break;
                                        }
                                        ValidWords.Add(formedWord);
                                        formedWord = "";
                                    }
                                    else if (formedWord != "" && errorMessage != true)
                                    {
                                        MessageBox.Show("Invalid Crozzle - Word not found in list.");
                                        errorMessage = true;
                                        fstr_out = new StreamWriter(@"../../LogFile.txt", true);
                                        fstr_out.WriteLine(txtFileNameSafe + "/" + csvFileNameSafe + " - Error: Invalid Crozzle - Word not found in list.");
                                        fstr_out.Close();
                                        break;
                                    }
                                    if (errorMessage != true)
                                    {
                                        if (letterBelowCount < 1 && letterAboveCount < 1)
                                        {
                                            intersectHEx = true;
                                        }
                                        else
                                        {
                                            letterBelowCount = 0;
                                            letterAboveCount = 0;
                                        }
                                    }
                                }

                            }
                        }
                    }

                    #endregion

                    #region Vertical word validation
                    //n = i
                    //m = j
                    for (int j = 0; j < CrozzleArray.GetLength(1); j++)
                    {
                        for (int i = 0; i < CrozzleArray.GetLength(0); i++)
                        {
                            if ((string)CrozzleArray.GetValue(i, j) != " ")
                            {
                                #region Try Catches
                                letterCurrent = (string)CrozzleArray.GetValue(i, j);
                                try
                                {
                                    letterRight = (string)CrozzleArray.GetValue(i, j + 1);
                                }
                                catch
                                {
                                    letterRight = null;
                                }

                                try
                                {
                                    letterLeft = (string)CrozzleArray.GetValue(i, j - 1);
                                }
                                catch
                                {
                                    letterLeft = null;
                                }

                                try
                                {
                                    letterBelow = (string)CrozzleArray.GetValue(i + 1, j);
                                }
                                catch
                                {
                                    letterBelow = null;
                                }

                                try
                                {
                                    letterAbove = (string)CrozzleArray.GetValue(i - 1, j);
                                }
                                catch
                                {
                                    letterAbove = null;
                                }
                                #endregion

                                if (letterBelow != " ")
                                {
                                    for (int n = i; n < CrozzleArray.GetLength(1); n++)
                                    {
                                        letterCurrent = (string)CrozzleArray.GetValue(n, j);

                                        #region Try Catches
                                        try
                                        {
                                            letterRight = (string)CrozzleArray.GetValue(n, j + 1);
                                            if (!String.IsNullOrWhiteSpace(letterRight))
                                            {
                                                letterRightCount++;
                                            }
                                        }
                                        catch
                                        {
                                            letterRight = null;
                                        }

                                        try
                                        {
                                            letterLeft = (string)CrozzleArray.GetValue(n, j - 1);
                                            if (!String.IsNullOrWhiteSpace(letterLeft))
                                            {
                                                letterLeftCount++;
                                            }
                                        }
                                        catch
                                        {
                                            letterLeft = null;
                                        }

                                        try
                                        {
                                            letterBelow = (string)CrozzleArray.GetValue(n + 1, j);
                                        }
                                        catch
                                        {
                                            letterBelow = null;
                                        }

                                        try
                                        {
                                            letterAbove = (string)CrozzleArray.GetValue(n - 1, j);
                                        }
                                        catch
                                        {
                                            letterAbove = null;
                                        }
                                        #endregion

                                        //Checks if word is passing through more than two other words
                                        if (errorMessage != true)
                                        {
                                            if (letterRightCount >= 3 || letterLeftCount >= 3)
                                            {
                                                intersectEM = true;
                                            }
                                        }

                                        /*if (errorMessage != true)
                                        {
                                            if (letterRightCount < 1 || letterLeftCount < 1)
                                            {
                                                intersectHEx = true;
                                            }
                                        }*/

                                        if (!String.IsNullOrWhiteSpace(letterBelow))
                                        {
                                            formedWord = formedWord + letterCurrent;
                                        }
                                        else if (!String.IsNullOrWhiteSpace(letterAbove))
                                        {
                                            formedWord = formedWord + letterCurrent;
                                            i = n;
                                            break;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                    if (listA.Contains(formedWord) == true && errorMessage != true)
                                    {
                                        if (ValidWords.Contains(formedWord) == true)
                                        {
                                            MessageBox.Show("Invalid Crozzle - Words have been used more than once.");
                                            errorMessage = true;
                                            fstr_out = new StreamWriter(@"../../LogFile.txt", true);
                                            fstr_out.WriteLine(txtFileNameSafe + "/" + csvFileNameSafe + " - Error: Invalid Crozzle - Words have been used more than once.");
                                            fstr_out.Close();
                                            break;
                                        }
                                        ValidWords.Add(formedWord);
                                        formedWord = "";
                                    }
                                    else if (formedWord != "" && errorMessage != true)
                                    {
                                        MessageBox.Show("Invalid Crozzle - Word not found in list.");
                                        errorMessage = true;
                                        fstr_out = new StreamWriter(@"../../LogFile.txt", true);
                                        fstr_out.WriteLine(txtFileNameSafe + "/" + csvFileNameSafe + " - Error: Invalid Crozzle - Word not found in list.");
                                        fstr_out.Close();
                                        break;
                                    }
                                    if (errorMessage != true)
                                    {
                                        if (letterRightCount < 1 && letterLeftCount < 1)
                                        {
                                            intersectHEx = true;
                                        }
                                        else
                                        {
                                            letterRightCount = 0;
                                            letterLeftCount = 0;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region Constraint Validation
                    string difficulty;
                    foreach (string item in listB.Skip(3))
                    {
                        difficulty = item;
                        if (intersectEM == true)
                        {
                            switch (difficulty)
                            {
                                case "EASY":
                                    MessageBox.Show("Invalid Crozzle - A word passes through more than two other words");
                                    errorMessage = true;
                                    fstr_out = new StreamWriter(@"../../LogFile.txt", true);
                                    fstr_out.WriteLine(txtFileNameSafe + "/" + csvFileNameSafe + " - Error: Invalid Crozzle - A word passes through more than two other words");
                                    fstr_out.Close();
                                    break;

                                case "MEDIUM":
                                    MessageBox.Show("Invalid Crozzle - A word passes through more than two other words");
                                    errorMessage = true;
                                    fstr_out = new StreamWriter(@"../../LogFile.txt", true);
                                    fstr_out.WriteLine(txtFileNameSafe + "/" + csvFileNameSafe + " - Error: Invalid Crozzle - A word passes through more than two other words");
                                    fstr_out.Close();
                                    break;
                            }
                        }

                        if (intersectHEx == true)
                            {
                                switch (difficulty)
                                {
                                case "HARD":
                                    MessageBox.Show("Invalid Crozzle - A word does not pass through at least one other word");
                                    errorMessage = true;
                                    fstr_out = new StreamWriter(@"../../LogFile.txt", true);
                                    fstr_out.WriteLine(txtFileNameSafe + "/" + csvFileNameSafe + " - Error: Invalid Crozzle - A word passes through more than two other words");
                                    fstr_out.Close();
                                    break;

                                case "EXTREME":
                                    MessageBox.Show("Invalid Crozzle - A word does not pass through at least one other word");
                                    errorMessage = true;
                                    fstr_out = new StreamWriter(@"../../LogFile.txt", true);
                                    fstr_out.WriteLine(txtFileNameSafe + "/" + csvFileNameSafe + " - Error: Invalid Crozzle - A word passes through more than two other words");
                                    fstr_out.Close();
                                    break;
                                }
                            }
                        }
                    
                    #endregion

                    //Crozzle Scoring
                    #region Scoring
                    score = 0;
                    string caseSwitch;
                    int p = 0;
                    int q = 0;
                    char[,] ScoringArray = new char[2, 26];
                    for (char c = 'A'; c <= 'Z'; c++)
                    {
                        while (q < ScoringArray.GetLength(0))
                        {
                            while (p < ScoringArray.GetLength(1))
                            {
                                ScoringArray[q, p] = c;
                                p++;
                                break;
                            }
                            break;
                        }
                    }

                    q++;
                    p = 0;

                    for (int c = 1; c <= 26; c++)
                    {
                        while (q < ScoringArray.GetLength(0))
                        {
                            while (p < ScoringArray.GetLength(1))
                            {
                                ScoringArray[q, p] = Convert.ToChar(c);
                                p++;
                                break;
                            }
                            break;
                        }
                    }



                    foreach (string item in listB.Skip(3))
                    {
                        caseSwitch = item;
                        switch (caseSwitch)
                        {
                            //Difficulty
                            #region Easy
                            case "EASY":
                                for (int listItem = 0; listItem < CrozzleLetters.Count; listItem++)
                                {
                                    if (CrozzleLetters[listItem] == " ")
                                    {
                                        CrozzleLetters.RemoveAt(listItem);
                                        listItem--;
                                    }
                                }
                                score = CrozzleLetters.Count;
                                break;
                            #endregion
                            #region Medium
                            case "MEDIUM":
                                int i = 0;
                                int j = 0;
                                for (int listItem = 0; listItem < CrozzleLetters.Count; listItem++)
                                {
                                    if (CrozzleLetters[listItem] == " ")
                                    {
                                        CrozzleLetters.RemoveAt(listItem);
                                        listItem--;
                                    }
                                }

                                for (int listItem = 0; listItem < CrozzleLetters.Count; listItem++)
                                {
                                    while (i < ScoringArray.GetLength(0))
                                    {
                                        while (j < ScoringArray.GetLength(1))
                                        {
                                            if (ScoringArray[i, j] == Convert.ToChar(CrozzleLetters[listItem]))
                                            {
                                                score = score + ScoringArray[i + 1, j];
                                                j = 0;
                                                break;
                                            }
                                            j++;
                                        }
                                        break;
                                    }
                                }
                                break;
                            #endregion
                            #region Hard
                            case "HARD":
                                int t = 0;
                                int l = 0;
                                for (int listItem = 0; listItem < CrozzleLetters.Count; listItem++)
                                {
                                    if (CrozzleLetters[listItem] == " ")
                                    {
                                        CrozzleLetters.RemoveAt(listItem);
                                        listItem--;
                                    }
                                }

                                for (int listItem = 0; listItem < CrozzleLetters.Count; listItem++)
                                {
                                    while (t < ScoringArray.GetLength(0))
                                    {
                                        while (l < ScoringArray.GetLength(1))
                                        {
                                            if (ScoringArray[t, l] == Convert.ToChar(CrozzleLetters[listItem]))
                                            {
                                                score = score + ScoringArray[t + 1, l];
                                                l = 0;
                                                break;
                                            }
                                            l++;
                                        }
                                        break;
                                    }
                                }

                                foreach (string scoredWord in ValidWords)
                                {
                                    score = score + 10;
                                }

                                break;
                            #endregion
                            #region Extreme
                            case "EXTREME":
                                int u = 0;
                                int o = 0;
                                for (int listItem = 0; listItem < CrozzleLetters.Count; listItem++)
                                {
                                    if (CrozzleLetters[listItem] == " ")
                                    {
                                        CrozzleLetters.RemoveAt(listItem);
                                        listItem--;
                                    }
                                }

                                for (int listItem = 0; listItem < CrozzleLetters.Count; listItem++)
                                {
                                    while (u < ScoringArray.GetLength(0))
                                    {
                                        while (o < ScoringArray.GetLength(1))
                                        {
                                            if (ScoringArray[u, o] == Convert.ToChar(CrozzleLetters[listItem]))
                                            {
                                                score = score + ScoringArray[u + 1, o];
                                                o = 0;
                                                break;
                                            }
                                            o++;
                                        }
                                        break;
                                    }
                                }

                                foreach (string scoredWord in ValidWords)
                                {
                                    score = score + 10;
                                }
                                break;
                            #endregion
                        }
                    }
                    #endregion

                }
                if (errorMessage != true)
                {
                    //Use CrozzleArray to form DataGridView that is displayed on GUI and populate list from CSV
                    listBox1.DataSource = listA;
                    
                    for (int rowIndex = 0; rowIndex < arrayRow; rowIndex++)
                    {
                        var row = new DataGridViewRow();
                        var column = new DataGridViewColumn();

                        for (int columnIndex = 0; columnIndex < arrayColumn; columnIndex++)
                        {
                            row.Cells.Add(new DataGridViewTextBoxCell()
                            {
                                Value = CrozzleArray[rowIndex, columnIndex]
                            });

                        }
                        dataGridView1.ColumnCount = arrayColumn;
                        dataGridView1.Rows.Add(row);
                    }
                }
        #endregion
            }
            if (errorMessage == true)
            {
                score = 0;
                fstr_out = new StreamWriter(@"../../LogFile.txt", true);
                fstr_out.WriteLine("End Report");
                fstr_out.WriteLine("Date: {0:hh:mm dd-MM-yyyy}", DateTime.Now);
                fstr_out.WriteLine(" ");
                fstr_out.WriteLine(" ");
                fstr_out.Close();
            }
            label1.Text = "Score: " + score.ToString();
        }
    }
}
