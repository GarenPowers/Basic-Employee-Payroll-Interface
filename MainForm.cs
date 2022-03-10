using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Powers_CourseProject_part2
{
    public partial class MainForm : Form
    {
        // class level references
        private const String FILENAME = "Employees.csv";

        public MainForm()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            // add item to the employee listbox
            InputForm1 frmInput = new InputForm1();

            using (frmInput)
            {
                frmInput.StartPosition = FormStartPosition.CenterParent;
                DialogResult result = frmInput.ShowDialog();

                //if cancelled, stop the method
                if (result == DialogResult.Cancel)
                    return;     // ends the method

                // get Employee info
                string fName = frmInput.FirstNameTextBox.Text;
                string lName = frmInput.LastNameTextBox.Text;
                string ssn = frmInput.SSNTextBox.Text;
                string date = frmInput.HireDateTextBox.Text;
                DateTime hireDate = DateTime.Parse(date);
                string healthIns = frmInput.healthinsuranceTextBox.Text;
                double lifeIns = Convert.ToDouble(frmInput.lifeinsuranceTextBox.Text);
                int vacation = Convert.ToInt32(frmInput.vacationTextBox.Text);

                Benefits benefits = new Benefits(healthIns, lifeIns, vacation);
                Employee emp = null;
                if( frmInput.hourlyRadioButton.Checked )
                {
                    double hourlyRate = Double.Parse(frmInput.hourlyRateTextBox.Text);
                    double hoursWorked = Double.Parse(frmInput.hoursWorkedTextBox.Text);
                    emp = new Hourly(fName, lName, ssn, hireDate, 
                        benefits, hourlyRate, hoursWorked);
                }
                else if( frmInput.salaryRadioButton.Checked)
                {
                    double annualSalary = Double.Parse(frmInput.salaryTextBox.Text);
                    emp = new Salary(fName, lName, ssn, hireDate,
                        benefits, annualSalary);
                }
                else
                {
                    MessageBox.Show("Eror. Employee type must be selected");
                    return; // end the method
                }

                EmployeesListBox.Items.Add(emp);

                // save the employee records to a file
                WriteEmpsToFile();
            }
        }

        private void WriteEmpsToFile()
        {
            List<Employee> empList = new List<Employee>();

            foreach (Employee emp in EmployeesListBox.Items)
            {
                empList.Add(emp);
            }

            //open a pipe to the file and create a translator
            FileStream fs = new FileStream(FILENAME, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();

            // write the entire object to the file in one line
            formatter.Serialize(fs, empList);

            // close the pipe
            fs.Close();
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            // remove the selected item from the employee listbox
            int itemNumber = EmployeesListBox.SelectedIndex;

            if (itemNumber > -1)
            {
                EmployeesListBox.Items.RemoveAt(itemNumber);
                WriteEmpsToFile();
            }
            else
            {
                MessageBox.Show("Please select employee to remove.");
            }

        }

        private void DisplayButton_Click(object sender, EventArgs e)
        {
            ReadEmpsFromFile();
        }
        private void ReadEmpsFromFile()
        {
            // clear the listbox
            EmployeesListBox.Items.Clear();

            // check to see if the file exists
            if( File.Exists(FILENAME) && new FileInfo(FILENAME).Length > 0 )
            {
                // create a pipe from the file and create a "translator"
                FileStream fs = new FileStream(FILENAME, FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();

                // read the Employee list from the file
                List<Employee> list = (List<Employee>)formatter.Deserialize(fs);


                // close the pipe
                fs.Close();

                // copy each Employee object to the listbox
                foreach (Employee emp in list)
                {
                    EmployeesListBox.Items.Add(emp);
                }
            }
        }

        private void PrintPaychecksButton_Click(object sender, EventArgs e)
        {
           foreach( Employee emp in EmployeesListBox.Items )
            {
                string output = emp.FirstName + " " + emp.LastName + "\n";
                output += "SSN: " + emp.SSN + "\n";
                output += "Hire Date: " + emp.HireDate.ToShortDateString() + "\n";
                output += "Pay Amount: " + emp.CalculatePay().ToString("C2");

                MessageBox.Show(output);
            }
        }

        private void EmployeesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Employee emp = (Employee)EmployeesListBox.SelectedItem;

            if( emp !=null )
            {
                InputForm1 updateForm = new InputForm1();


                updateForm.Text = "Update Employee Information";
                updateForm.SubmitButton.Text = "Update";
                updateForm.StartPosition = FormStartPosition.CenterParent;
                updateForm.FirstNameTextBox.Text = emp.FirstName;
                updateForm.LastNameTextBox.Text = emp.LastName;
                updateForm.SSNTextBox.Text = emp.SSN;
                updateForm.HireDateTextBox.Text = emp.HireDate.ToShortDateString();
                updateForm.healthinsuranceTextBox.Text = emp.Benefits.HealthInsurance;
                updateForm.lifeinsuranceTextBox.Text = emp.Benefits.LifeInsurance.ToString("f2");
                updateForm.vacationTextBox.Text = emp.Benefits.Vacation.ToString();
                DialogResult result = updateForm.ShowDialog();

                // if cancelled, stop the method
                if (result == DialogResult.Cancel)
                    return;     // ends the method


                // delete selected object
                int position = EmployeesListBox.SelectedIndex;
                EmployeesListBox.Items.RemoveAt(position);

                // create new employee using the update information
                Employee newEmp = new Employee();
                newEmp.FirstName = updateForm.FirstNameTextBox.Text;
                newEmp.LastName = updateForm.LastNameTextBox.Text;
                newEmp.SSN = updateForm.SSNTextBox.Text;
                newEmp.HireDate = DateTime.Parse(updateForm.HireDateTextBox.Text);
                newEmp.Benefits.HealthInsurance = 
                    updateForm.healthinsuranceTextBox.Text;
                newEmp.Benefits.LifeInsurance =
                    Double.Parse(updateForm.lifeinsuranceTextBox.Text);
                newEmp.Benefits.Vacation =
                    int.Parse(updateForm.vacationTextBox.Text);

                // add new employee to the listbox
                EmployeesListBox.Items.Add(newEmp);
            }
        }
    }
}
