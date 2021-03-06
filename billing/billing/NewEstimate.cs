﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace billing
{
    public partial class NewEstimate : Form
    {
        private bool CheckedFlag;
        private int i;
        private int l;
        private bool FormLoadFlag = false;
        private bool EditFormFlag = false;
        private DataTable customerData;
        private DataTable vehicledata;
        private DataTable itemsData;
        private DataTable laboursdata;
        private string vehicleno;
        private decimal Temp;
        private DataTable dataItemsTable;
        private DataTable dataLaborTable;
        private string p;

        private void getCustomerData()
        {
            ClassDatabaseConnection DatabaseConnectObj = new ClassDatabaseConnection();
            try
            {
                DatabaseConnectObj.SqlQuery("SELECT CustomerId, CustomerName, CustomerNo, VehicleNo, VehicleId FROM dbo.Customer");
                customerData = DatabaseConnectObj.ExecuteQuery();
            }
            catch (Exception ex)
            {
                string output = ex.Message + " getCustomerData"; MessageBox.Show(output);
            }
            finally
            {
                DatabaseConnectObj.DatabaseConnectionClose();
            }
        }

        private void getLaboursData()
        {
            ClassDatabaseConnection DatabaseConnectObj = new ClassDatabaseConnection();
            try
            {
                DatabaseConnectObj.SqlQuery("SELECT LabourId, LabourName, LabourDesc, LabourPrice FROM Labour");
                laboursdata = DatabaseConnectObj.ExecuteQuery();
            }
            catch (Exception ex)
            {
                string output = ex.Message + " getLaboursData"; MessageBox.Show(output);
            }
            finally
            {
                DatabaseConnectObj.DatabaseConnectionClose();
            }
        }

        private void getItemsData()
        {
            ClassDatabaseConnection DatabaseConnectObj = new ClassDatabaseConnection();
            try
            {
                DatabaseConnectObj.SqlQuery("SELECT ItemId, ItemName, ItemDesc, ItemPrice, VehicleId FROM dbo.Items");
                itemsData = DatabaseConnectObj.ExecuteQuery();
            }
            catch (Exception ex)
            {
                string output = ex.Message + " getItemsData"; MessageBox.Show(output);
            }
            finally
            {
                DatabaseConnectObj.DatabaseConnectionClose();
            }
        }

        private void getVehicleData()
        {
            ClassDatabaseConnection DatabaseConnectObj = new ClassDatabaseConnection();
            try
            {
                DatabaseConnectObj.SqlQuery("SELECT Id, VehicleType, VehicleName FROM dbo.Vehicle");
                vehicledata = DatabaseConnectObj.ExecuteQuery();
            }
            catch (Exception ex)
            {
                string output = ex.Message + " getVehicleData"; MessageBox.Show(output);
            }
            finally
            {
                DatabaseConnectObj.DatabaseConnectionClose();
            }
        }

        public void loadComboBoxItemName(DataTable customerTable, DataTable vehicleTable, DataTable itemsTable)
        {
            ComboBoxItemName.Text = "";
            ComboBoxItemName.Items.Clear();
            try
            {
                try
                {
                    DataRow[] vehicleRowData = vehicleTable.Select("VehicleType='" + ComboBoxVehicleModel.Text.Trim() + "' AND VehicleName='" + ComboBoxVehicleName.Text.Trim() + "'");
                    string selectedVehicleId = vehicleRowData[0]["Id"].ToString().Trim();
                    DataRow[] itemsData = itemsTable.Select("VehicleId = '" + selectedVehicleId + "'");

                    foreach (DataRow row in itemsData)
                    {
                        ComboBoxItemName.Items.Add(row["ItemName"].ToString().Trim());
                    }
                    if (ComboBoxItemName.Items.Count > 0)
                    {
                        ComboBoxItemName.Text = ComboBoxItemName.Items[0].ToString().Trim();
                    }
                }
                catch (Exception ex)
                {
                    string output = ex.Message + " loadComboBoxItemName"; MessageBox.Show(output);
                }
            }
            catch (Exception ex)
            {
                string output = ex.Message + " loadComboBoxItemName"; MessageBox.Show(output);
            }
        }

        public void loadComboBoxLabourName(DataTable LabourTable)
        {
            ComboBoxLabourName.Text = "";
            ComboBoxLabourName.Items.Clear();

            try
            {
                foreach (DataRow row in LabourTable.Rows)
                {
                    ComboBoxLabourName.Items.Add(row["LabourName"].ToString().Trim());
                }
                if (ComboBoxLabourName.Items.Count > 0)
                {
                    ComboBoxLabourName.Text = ComboBoxLabourName.Items[0].ToString().Trim();
                }
            }
            catch (Exception ex)
            {
                string output = ex.Message + " loadComboBoxLabourName"; MessageBox.Show(output);
            }
        }

        private void loadTotalAndSubTotal()
        {
            Decimal tempTotal = 0;
            Decimal tempSubTotal = 0;
            foreach (DataGridViewRow row in dataGridItems.Rows)
            {
                tempTotal += Convert.ToDecimal(row.Cells["total"].Value);
                tempSubTotal += Convert.ToDecimal(row.Cells["Quantity"].Value) * Convert.ToDecimal(row.Cells["UnitPrice"].Value);
            }
            foreach (DataGridViewRow row in DataGridLabour.Rows)
            {
                tempTotal += Convert.ToDecimal(row.Cells["LabourTotal"].Value);
                tempSubTotal += Convert.ToDecimal(row.Cells["LabourCharge"].Value);
            }
            TextBoxSubTotal.Text = tempSubTotal.ToString();
            TextBoxTotal.Text = tempTotal.ToString();
        }

        private void loadVehiclefield()
        {
            //move the vehicledata to a parent place so that the sql query shouldnt be executed every time a new customer is added
            ComboBoxVehicleModel.Items.Clear();
            try
            {
                DataTable VehicleTypeTable = vehicledata.DefaultView.ToTable(true, "VehicleType");
                foreach (DataRow row in VehicleTypeTable.Rows)
                {
                    ComboBoxVehicleModel.Items.Add(row["VehicleType"].ToString().Trim());
                }
                DataTable VehicleNameTable = vehicledata.DefaultView.ToTable(true, "VehicleName");
                foreach (DataRow row in VehicleNameTable.Rows)
                {
                    ComboBoxVehicleName.Items.Add(row["VehicleName"].ToString().Trim());
                }
            }
            catch (Exception ex)
            {
                string output = ex.Message + " loadVehiclefield"; MessageBox.Show(output);
            }
        }

        public void LoadCusCombobox()
        {
            ComboBoxClientName.Items.Clear();
            try
            {
                foreach (DataRow row in customerData.Rows)
                {
                    ComboBoxClientName.Items.Add(row["CustomerName"].ToString().Trim() + " ( " + row["VehicleNo"].ToString().Trim() + " )");
                }
            }
            catch (Exception ex)
            {
                string output = ex.Message + " LoadCusCombobox"; MessageBox.Show(output);
            }
        }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape) this.Close();
            bool res = base.ProcessCmdKey(ref msg, keyData);
            return res;
        }

        //LinkedList<Decimal> listUnitPrice = new LinkedList<Decimal>();
        public NewEstimate()
        {
            InitializeComponent();
            FormLoadFlag = true;
        }

        public NewEstimate(DataTable dataItemsTable, DataTable dataLaborTable, string st)
        {
            InitializeComponent();
            FormLoadFlag = true;
            dataGridItems.DataSource = dataItemsTable;
            DataGridLabour.DataSource = dataLaborTable;
            EditFormFlag = true;
            ComboBoxClientName.Text = st.Split(',').ElementAt(0).Trim();
            LabelHidden.Text = st.Split(',').ElementAt(1);
            ComboBoxVehicleModel.Text = st.Split(' ').ElementAt(2);
        }

        private void ComboBoxClientName_SelectedIndexChanged(object sender, EventArgs e)
        {
            String[] substrings = ComboBoxClientName.Text.Split('(');
            string CustomerName = substrings[0];

            setVehicleNo(substrings[1]);

            //load the customers vehicle
            DataRow[] customerRowData = customerData.Select("VehicleNo = '" + vehicleno + "'");
            DataRow[] vehicleRowData = vehicledata.Select("Id = '" + customerRowData[0]["VehicleId"].ToString().Trim() + "'");
            ComboBoxVehicleModel.Text = vehicleRowData[0]["VehicleType"].ToString().Trim();
            ComboBoxVehicleName.Text = vehicleRowData[0]["VehicleName"].ToString().Trim();
            string temp = customerRowData[0]["CustomerNo"].ToString().Trim() + " " + customerRowData[0]["VehicleNo"].ToString().Trim() + "\n" + vehicleRowData[0]["VehicleType"].ToString().Trim() + " " + vehicleRowData[0]["VehicleName"].ToString().Trim();
            LabelHidden.Text = temp;

        //check if the customer is from database
            if (ComboBoxClientName.Items.Contains(ComboBoxClientName.Text)) 
            {
                try // load hidden textox and check if a message is scheduled aand load the desc combo box
                {
                    
                    ClassDatabaseConnection DatabaseConnectObj = new ClassDatabaseConnection();
                   
                    try // check if a message is scheduled
                    {
                        DataTable dt = new DataTable();
                        DatabaseConnectObj.SqlQuery("SELECT VehicleNo FROM Schedule WHERE (VehicleNo = '" + vehicleno + "')");
                        dt = DatabaseConnectObj.ExecuteQuery();
                        DataRow row = dt.Rows[0];
                        if (!row["VehicleNo"].Equals(""))
                        {
                            CheckBoxSchedule.Checked = false;
                            CheckedFlag = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.Message.ToString().Equals("There is no row at position 0."))
                        {
                            CheckBoxSchedule.Checked = true;
                        }
                        else
                        {
                            string output = ex.Message + " ComboBoxClientName_SelectedIndexChanged"; MessageBox.Show(output);
                        }
                    }
                    finally
                    {
                        DatabaseConnectObj.DatabaseConnectionClose();
                    }
                }
                catch (Exception ex)
                {
                    string output = ex.Message + " ComboBoxClientName_SelectedIndexChanged"; MessageBox.Show(output);
                }
            }
            else
            {
                MessageBox.Show("Customer Does Not Exist,please create add the customer");
            }
        }

        private void setVehicleNo(string input)
        {
            vehicleno = input.Split(')').ElementAt(0).Trim();
        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            if (ComboBoxItemName.Items.Contains(ComboBoxItemName.Text))
            {
                DataRow[] vehicleRowData = vehicledata.Select("VehicleType='" + ComboBoxVehicleModel.Text.Trim() + "' AND VehicleName='" + ComboBoxVehicleName.Text.Trim() + "'");
                string selectedVehicleId = vehicleRowData[0]["Id"].ToString().Trim();
                DataRow[] itemRowData = itemsData.Select("VehicleId = '" + selectedVehicleId + "' AND ItemName = '" + ComboBoxItemName.Text.Trim() + "'");
                string itemid1 = itemRowData[0]["ItemId"].ToString().Trim();
                if (EditFormFlag == true)
                {
                    DataTable dataTable = (DataTable)dataGridItems.DataSource;
                    DataRow drToAdd = dataTable.NewRow();
                    drToAdd["ItemDesc"] = ComboBoxItemName.Text.ToString();
                    drToAdd["Quantity"] = NumericQuantity.Value.ToString();
                    drToAdd["price"] = NumericUnitPrice.Value.ToString();
                    drToAdd["Tax"] = ComboboxItemTax.Text;
                    drToAdd["ItemNo"] = itemid1;
                    dataTable.Rows.Add(drToAdd);
                    dataTable.AcceptChanges();
                    TextBoxSubTotal.Text = (Convert.ToInt32(TextBoxSubTotal.Text) + (NumericUnitPrice.Value * NumericQuantity.Value)).ToString();
                    TextBoxTotal.Text = (Convert.ToDecimal(TextBoxTotal.Text) + NumericQuantity.Value * (NumericUnitPrice.Value + (NumericUnitPrice.Value * (Convert.ToDecimal(ComboboxItemTax.Text) / 100)))).ToString();
                }
                else
                {
                    decimal unitprice = Math.Round(NumericUnitPrice.Value, 2);
                    decimal quantity = NumericQuantity.Value;
                    //calculate total without tax
                    decimal total = (quantity * unitprice);
                    //add tax
                    total = Math.Round(total + (total * (Convert.ToDecimal(ComboboxItemTax.Text) / 100)), 2);
                    //add to data grid
                    dataGridItems.Rows.Add(ComboBoxItemName.Text.ToString(), quantity.ToString(), unitprice.ToString(), ComboboxItemTax.Text, total.ToString(), itemid1);
                    //update sub total
                    TextBoxSubTotal.Text = Math.Round((Convert.ToDecimal(TextBoxSubTotal.Text) + (unitprice * quantity)), 2).ToString();
                    //update total
                    TextBoxTotal.Text = Math.Round((Convert.ToDecimal(TextBoxTotal.Text) + quantity * (unitprice + (unitprice * (Convert.ToDecimal(ComboboxItemTax.Text) / 100)))), 2).ToString();
                }
                ComboBoxItemName.Focus();
                ButtonSave.Enabled = true;
            }
            else
            {
                MessageBox.Show("Please add the new item before adding this to the Estimate");
                ComboBoxItemName.Focus();
            }
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            int Estimateid = 0;
            if (ComboBoxClientName.Items.Contains(ComboBoxClientName.Text)) // check if customer is selected
            {
                try // select customer id
                {
                    ClassDatabaseConnection DatabaseConnectObj = new ClassDatabaseConnection();
                    DataTable dt1 = new DataTable();
                    string cusid1 = "";
                    string vehicleno = "";
                    string[] substrings = ComboBoxClientName.Text.Split('(');
                    vehicleno = substrings[1].Split(')').ElementAt(0).Trim();
                    DatabaseConnectObj.SqlQuery("SELECT CustomerId FROM Customer WHERE (VehicleNo = '" + vehicleno + "')");
                    dt1 = DatabaseConnectObj.ExecuteQuery();
                    foreach (DataRow row in dt1.Rows)
                    {
                        cusid1 = row["CustomerId"].ToString().Trim();
                    }
                    try // insert Estimate first
                    {
                        string date = DateTimePickerIssued.Value.Month.ToString() + "/" + DateTimePickerIssued.Value.Day.ToString() + "/" + DateTimePickerIssued.Value.Year.ToString();
                        DataTable dt = new DataTable();
                        decimal TempTotal = 0;
                        for (int i = 0; i < dataGridItems.RowCount; i++)
                        {
                            decimal total = (Convert.ToDecimal(dataGridItems.Rows[i].Cells["Quantity"].Value) * Convert.ToDecimal(dataGridItems.Rows[i].Cells["UnitPrice"].Value));
                            total = total + (total * (Convert.ToDecimal(dataGridItems.Rows[i].Cells["Tax"].Value) / 100));
                            TempTotal += total;
                        }
                        for (int i = 0; i < DataGridLabour.RowCount;i++)
                        {
                            decimal labourTotal = (Convert.ToDecimal(DataGridLabour.Rows[i].Cells["LabourTotal"].Value));
                            TempTotal += labourTotal;
                        }
                            DatabaseConnectObj.SqlQuery("INSERT INTO Estimate (EstimateNo, CustomerId, Date, total, remark) VALUES ('" + NumericEstimateNo.Text + "','" + cusid1 + "','" + date + "','" + TempTotal + "','" + TextBoxRemark.Text + "')");
                        DatabaseConnectObj.ExecutNonQuery();
                    }
                    catch (Exception ex)
                    {
                        string output = ex.Message + " insert Estimate first ButtonSave_Click"; MessageBox.Show(output);
                    }

                    try //select Estimateid to insert Estimate with multiple Estimate no.
                    {
                        DataTable dt = new DataTable();
                        DatabaseConnectObj.SqlQuery("SELECT MAX(Estimateid) AS Estimateid FROM Estimate ORDER BY Estimateid");
                        dt = DatabaseConnectObj.ExecuteQuery();
                        Estimateid = Convert.ToInt32(dt.Rows[0].ItemArray[0]);
                    }
                    catch (Exception ex)
                    {
                        string output = ex.Message + " multiple Estimate no ButtonSave_Click"; MessageBox.Show(output);
                    }
                    try // insert the bill items details
                    {
                        for (int i = 0; i < dataGridItems.Rows.Count; i++)
                        {
                            DatabaseConnectObj.SqlQuery("INSERT INTO BillItemDetailsEstimate (EstimateNo, ItemNo, Quantity, price, Tax, Estimateid) VALUES ('" + NumericEstimateNo.Value.ToString().Trim() + "','" + dataGridItems.Rows[i].Cells["Itemid"].Value.ToString().Trim() + "','" + dataGridItems.Rows[i].Cells["Quantity"].Value.ToString().Trim() + "','" + dataGridItems.Rows[i].Cells["UnitPrice"].Value.ToString().Trim() + "','" + dataGridItems.Rows[i].Cells["Tax"].Value.ToString().Trim() + "','" + Estimateid + "')");
                            DatabaseConnectObj.ExecutNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        string output = ex.Message + "items details ButtonSave_Click"; MessageBox.Show(output);
                    }
                    try // insert the bill labour details
                    {
                        for (int i = 0; i < DataGridLabour.Rows.Count; i++)
                        {
                            DatabaseConnectObj.SqlQuery("INSERT INTO BillLabourDetailEstimate (EstimateNo, LabourNo, price, Tax, Estimateid) VALUES ('" + NumericEstimateNo.Value.ToString().Trim() + "','" + DataGridLabour.Rows[i].Cells["LabourId"].Value.ToString().Trim() + "','" + DataGridLabour.Rows[i].Cells["LabourCharge"].Value.ToString().Trim() + "','" + DataGridLabour.Rows[i].Cells["LabourTax"].Value.ToString().Trim() + "','" + Estimateid + "')");
                            DatabaseConnectObj.ExecutNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        string output = ex.Message + "labour details ButtonSave_Click"; MessageBox.Show(output);
                    }
                    if (CheckBoxSchedule.Checked.Equals(true)) // if scheduled box is checked schedule the message
                    {
                        if (CheckedFlag == false)
                        {
                            try
                            {
                                string date = dateTimePickerSchedule.Value.Month.ToString() + "/" + dateTimePickerSchedule.Value.Day.ToString() + "/" + dateTimePickerSchedule.Value.Year.ToString();
                                DatabaseConnectObj.SqlQuery("INSERT INTO Schedule (VehicleNo, Date) VALUES ('" + vehicleno + "','" + date + "')");
                                DatabaseConnectObj.ExecutNonQuery();
                            }
                            catch (Exception ex)
                            {
                                string output = ex.Message + " schedule the message ButtonSave_Click"; MessageBox.Show(output);
                            }
                            finally
                            {
                                DatabaseConnectObj.DatabaseConnectionClose();
                            }
                        }
                        else
                        {
                            try
                            {
                                DatabaseConnectObj.SqlQuery("UPDATE Schedule SET Date = '" + dateTimePickerSchedule.Value.ToShortDateString() + "' WHERE (VehicleNo = '" + vehicleno + "')");
                                DatabaseConnectObj.ExecutNonQuery();
                            }
                            catch (Exception ex)
                            {
                                string output = ex.Message + " ButtonSave_Click"; MessageBox.Show(output);
                            }
                            finally
                            {
                                DatabaseConnectObj.DatabaseConnectionClose();
                            }
                        }
                    }
                    if (CheckBoxSendSms.Checked.Equals(true))
                    {
                       
                    }
                    DatabaseConnectObj.DatabaseConnectionClose();
                }
                catch (Exception ex)
                {
                    string output = ex.Message + " ButtonSave_Click"; MessageBox.Show(output);
                }
                ButtonSave.Enabled = false;
                ButtonAddLabour.Enabled = false;
                ButtonAdd.Enabled = false;
                ButtonPreview.Enabled = true;
            }
            else
            {
                MessageBox.Show("First add the client name.");
            }
        }


        private void ButtonPreview_Click(object sender, EventArgs e)
        {
            /*PrintPreviewEstimate.Document = PrintDocumentEstimate;
            PrintPreviewEstimate.Size = new System.Drawing.Size(595, 842); wt,ht
            PrintPreviewEstimate.ShowDialog();*/
            //PrintPreview PrintPreviewForm = new PrintPreview(PrintDocumentEstimate,TextBoxTotal.Text);
            //PrintPreviewForm.ShowDialog();
            using (var dlg = new CoolPrintPreviewDialog(null,TextBoxTotal.Text,LabelHidden.Text))
            {
                dlg.Document = PrintDocumentEstimate;
                dlg.ShowDialog(this);
            }
            
        }

        private void PrintDocumentEstimate_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            int Height = 80,Yincrement=20;
            string date = DateTimePickerIssued.Value.Month.ToString() + "/" + DateTimePickerIssued.Value.Day.ToString() + "/" + DateTimePickerIssued.Value.Year.ToString();
            Bitmap bmp = Properties.Resources.tecnocraft;
            Image CompanyLogo = bmp;
            Rectangle rect2 = new Rectangle(25, 400, 800, 50);
            String[] substrings = ComboBoxClientName.Text.Split('(');

            Pen pent = new Pen(Color.Black, 3);
            Point point1 = new Point(25, 375);
            Point point2 = new Point(820, 375);
            Font FontHeading = new Font("Helvetica", 25, FontStyle.Bold);
            Font FontTitle = new Font("Helvetica", 15, FontStyle.Bold);
            Font FontDesc = new Font("Helvetica", 12, FontStyle.Regular);
            Font FontTotal = new Font("Helvetica",12,FontStyle.Bold);

            //company details
            e.Graphics.DrawImage(CompanyLogo, 40, 50, 300, 40);
            e.Graphics.DrawString("Estimate : "+ NumericEstimateNo.Value.ToString(), FontHeading, Brushes.Black, new Point(530, 45));
            //print date

            //company address
            e.Graphics.DrawString("From ", FontDesc, Brushes.Gray, new Point(50, Height+Yincrement+10));
            e.Graphics.DrawString("#72, 7th Main, B.O.B Colony,", FontDesc, Brushes.Black, new Point(110, Height += Yincrement + 10));
            e.Graphics.DrawString("J.P.Nagar, 7th Phase, ", FontDesc, Brushes.Black, new Point(110, Height += Yincrement));
            e.Graphics.DrawString("Bangalore-78.", FontDesc, Brushes.Black, new Point(110, Height += Yincrement));
            e.Graphics.DrawString("Ph: 9886254448,9880369118", FontDesc, Brushes.Black, new Point(110, Height += Yincrement));
            // customer details
            e.Graphics.DrawString("To ", FontDesc, Brushes.Gray, new Point(530, (Height + Yincrement) - (Yincrement * 4)));
            e.Graphics.DrawString(substrings[0] +"\n"+LabelHidden.Text, FontDesc, Brushes.Black, new Point(560, Height-(Yincrement*3)));
            //Estimate no and date
            e.Graphics.DrawString("Issued ", FontDesc, Brushes.Gray, new Point(50, Height + Yincrement + 5));
            e.Graphics.DrawString(date.ToString(), FontDesc, Brushes.Black, new Point(110, Height += Yincrement+5));
            int j=i;
            int k = l;
            if (j != dataGridItems.RowCount)
            {
                //table
                e.Graphics.DrawString("#", FontTitle, Brushes.Black, new Point(35, Height += Yincrement + 20));
                e.Graphics.DrawString("Item Name", FontTitle, Brushes.Black, new Point(90, Height));
                e.Graphics.DrawString("Qty", FontTitle, Brushes.Black, new Point(335, Height));
                e.Graphics.DrawString("Price", FontTitle, Brushes.Black, new Point(435, Height));
                e.Graphics.DrawString("Tax", FontTitle, Brushes.Black, new Point(530, Height));
                e.Graphics.DrawString("Total", FontTitle, Brushes.Black, new Point(635, Height));
                e.Graphics.DrawLine(pent, new Point(30, Height += Yincrement + 10), new Point(800, Height));
                //table content

                for (j = i; j < dataGridItems.RowCount && Height < 920; j++)
                {
                    e.Graphics.DrawString((j + 1).ToString(), FontDesc, Brushes.Black, new Point(35, Height += Yincrement + 10));
                    e.Graphics.DrawString(dataGridItems.Rows[j].Cells["Desc"].Value.ToString(), FontDesc, Brushes.Black, new Point(90, Height));
                    e.Graphics.DrawString(dataGridItems.Rows[j].Cells["Quantity"].Value.ToString(), FontDesc, Brushes.Black, new Point(339, Height));
                    e.Graphics.DrawString(dataGridItems.Rows[j].Cells["UnitPrice"].Value.ToString(), FontDesc, Brushes.Black, new Point(435, Height));
                    e.Graphics.DrawString(dataGridItems.Rows[j].Cells["Tax"].Value.ToString(), FontDesc, Brushes.Black, new Point(535, Height));
                    e.Graphics.DrawString(dataGridItems.Rows[j].Cells["Total"].Value.ToString(), FontTotal, Brushes.Black, new Point(635, Height));
                }
            }
            else
            {
                //table
                e.Graphics.DrawString("#", FontTitle, Brushes.Black, new Point(35, Height += Yincrement + 20));
                e.Graphics.DrawString("Labor", FontTitle, Brushes.Black, new Point(90, Height));
                e.Graphics.DrawString("Charge", FontTitle, Brushes.Black, new Point(435, Height));
                e.Graphics.DrawString("Tax", FontTitle, Brushes.Black, new Point(530, Height));
                e.Graphics.DrawString("Total", FontTitle, Brushes.Black, new Point(635, Height));
                e.Graphics.DrawLine(pent, new Point(30, Height += Yincrement + 10), new Point(800, Height));
                //table content
                for (k = l; k < DataGridLabour.RowCount && Height < 920; k++)
                {
                    e.Graphics.DrawString((k + 1).ToString(), FontDesc, Brushes.Black, new Point(35, Height += Yincrement + 10));
                    e.Graphics.DrawString(DataGridLabour.Rows[k].Cells["LabourName"].Value.ToString(), FontDesc, Brushes.Black, new Point(90, Height));
                    e.Graphics.DrawString(DataGridLabour.Rows[k].Cells["LabourCharge"].Value.ToString(), FontDesc, Brushes.Black, new Point(435, Height));
                    e.Graphics.DrawString(DataGridLabour.Rows[k].Cells["LabourTax"].Value.ToString(), FontDesc, Brushes.Black, new Point(535, Height));
                    e.Graphics.DrawString(DataGridLabour.Rows[k].Cells["LabourTotal"].Value.ToString(), FontTotal, Brushes.Black, new Point(635, Height));
                }
            }
            if (j < dataGridItems.RowCount)
            {
                e.HasMorePages = true;
                i = j;
            }
            else if (k < DataGridLabour.RowCount)
            {
                e.HasMorePages = true;
                i = j;
                l = k;
            }
            else
            {
                e.HasMorePages = false;
                j = 0;
                k = 0;
                l = 0;
                i = 0;
            }
            //Estimate total
            e.Graphics.DrawLine(pent, new Point(30,960), new Point(800,960));
            e.Graphics.DrawString("Remark", FontTitle, Brushes.Black, new Point(30, 970));
            if (TextBoxRemark.TextLength > 50)
            {
                e.Graphics.DrawString(TextBoxRemark.Text.Substring(0,49), new Font("Calibri", 13, FontStyle.Bold), Brushes.Black, new Point(50, 1000));
                e.Graphics.DrawString(TextBoxRemark.Text.Substring(49, TextBoxRemark.Text.Length-49), new Font("Calibri", 13, FontStyle.Bold), Brushes.Black, new Point(50, 1020));
            }
            else
            {
                e.Graphics.DrawString(TextBoxRemark.Text, new Font("Calibri", 13, FontStyle.Bold), Brushes.Black, new Point(30, 1000));
            }
            e.Graphics.DrawString("SubTotal", FontTitle, Brushes.Black, new Point(550, 970));
            e.Graphics.DrawString(TextBoxSubTotal.Text, FontTitle, Brushes.Black, new Point(670, 970));
            e.Graphics.DrawString("Tax: ", FontTitle, Brushes.Black, new Point(550,1000));
            e.Graphics.DrawString((Convert.ToDecimal(TextBoxTotal.Text) - Convert.ToDecimal(TextBoxSubTotal.Text)).ToString(), FontTitle, Brushes.Black, new Point(670, 1000));
            e.Graphics.DrawString("Total", FontTitle, Brushes.Black, new Point(550, 1030));
            e.Graphics.DrawString(TextBoxTotal.Text, FontTitle, Brushes.Black, new Point(670, 1030));
        }

        private void PrintPreviewEstimate_FormClosed(object sender, FormClosedEventArgs e)
        {
            i = 0;
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //after add the value to the grid add to string or array or most probably linked list
            //then this function is called to change the total value in the grid and the total text box
            string columnName = dataGridItems.Columns[e.ColumnIndex].Name.ToString().Trim();

            if (FormLoadFlag == true && dataGridItems.Rows[e.RowIndex].Cells[columnName].Value != null)
            {
                string datagridtax = dataGridItems.Rows[e.RowIndex].Cells[columnName].Value.ToString();
                string datagridunitprice = dataGridItems.Rows[e.RowIndex].Cells["UnitPrice"].Value.ToString(); ;
                if (datagridtax.EndsWith("q"))
                {
                    dataGridItems.Rows[e.RowIndex].Cells["UnitPrice"].Value = Math.Round((Convert.ToDecimal(datagridunitprice)) / ((Convert.ToDecimal(datagridtax.Split('q')[0]) / 100) + 1), 2);
                    dataGridItems.Rows[e.RowIndex].Cells[columnName].Value = datagridtax.Split('q')[0];
                }
                if (Convert.ToDecimal(dataGridItems.Rows[e.RowIndex].Cells[e.ColumnIndex].Value) < Temp)
                {
                    DialogResult result = MessageBox.Show("Do you want to save changes?", "Confirmation", MessageBoxButtons.YesNo);
                    if (result == DialogResult.No)
                    {
                        dataGridItems.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Temp;
                    }
                }
                decimal total = (Convert.ToDecimal(dataGridItems.Rows[e.RowIndex].Cells["Quantity"].Value) * Convert.ToDecimal(dataGridItems.Rows[e.RowIndex].Cells["UnitPrice"].Value));
                total = total + (total * (Convert.ToDecimal(dataGridItems.Rows[e.RowIndex].Cells["Tax"].Value) / 100));
                dataGridItems.Rows[e.RowIndex].Cells["Total"].Value = total;
                loadTotalAndSubTotal();
            }
            else if (FormLoadFlag == true && dataGridItems.Rows[e.RowIndex].Cells["UnitPrice"].Value == null)
            {
                dataGridItems.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Temp;
            }
        }

        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            try
            {
                Decimal subtotal = Convert.ToInt32(e.Row.Cells["UnitPrice"].Value) * Convert.ToInt32(e.Row.Cells["Quantity"].Value);
                Decimal total = Convert.ToDecimal(e.Row.Cells["Total"].Value);
                TextBoxSubTotal.Text = (Convert.ToInt32(TextBoxSubTotal.Text) - subtotal).ToString();
                TextBoxTotal.Text = (Convert.ToDecimal(TextBoxTotal.Text) - total).ToString();
            }
            catch (Exception ex)
            {
                string output = ex.Message + " dataGridView1_UserDeletingRow"; MessageBox.Show(output);
            }
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            Temp = Convert.ToDecimal(dataGridItems.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
        }


        private void TextBoxTotal_TextChanged(object sender, EventArgs e)
        {
            TextBoxPaid.Text = TextBoxTotal.Text;
        }

        private void TextBoxRemark_TextChanged(object sender, EventArgs e)
        {
            if(TextBoxRemark.TextLength>100)
            {
                MessageBox.Show("remark cannot exced 100 characters");
                TextBoxRemark.Text = "";
            }
        }

        private void ComboBoxVehicleModel_Leave(object sender, EventArgs e)
        {
            if (!ComboBoxVehicleModel.Items.Contains(ComboBoxVehicleModel.Text.Trim()))
            {
                DialogResult result = MessageBox.Show("Selected Model does not exist, Do you want to add it?", "Delete Confirmation", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    string newVehicleType = ComboBoxVehicleModel.Text.Trim();
                    try
                    {
                        ClassDatabaseConnection DatabaseConnectObj = new ClassDatabaseConnection();
                        try
                        {
                            DatabaseConnectObj.SqlQuery("INSERT INTO Vehicle (VehicleType) VALUES ('" + newVehicleType + "')");
                            DatabaseConnectObj.ExecutNonQuery();
                        }
                        catch (Exception ex)
                        {
                            string output = ex.Message + " ComboBoxVehicleModel_Leave"; MessageBox.Show(output);
                        }
                        finally
                        {
                            DatabaseConnectObj.DatabaseConnectionClose();
                        }
                    }
                    catch (Exception ex)
                    {
                        string output = ex.Message + " ComboBoxVehicleModel_Leave"; MessageBox.Show(output);
                    }
                    ComboBoxVehicleModel.Items.Add(ComboBoxVehicleModel.Text);
                }
                else
                {
                    ComboBoxVehicleModel.Text = "";
                }
            }
        }

        private void NewEstimate_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = char.ToUpper(e.KeyChar);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NewCustomer NewCustomerObj = new NewCustomer();
            NewCustomerObj.ShowDialog();
            getCustomerData();
            LoadCusCombobox();
            getVehicleData();
            loadVehiclefield();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            NewService NewServiceObj = new NewService(ComboBoxVehicleModel.Text.Trim(), ComboBoxVehicleName.Text.Trim());
            NewServiceObj.ShowDialog();
            getItemsData();
        }

        private void ComboBoxClientName_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = char.ToUpper(e.KeyChar);
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void label14_Click_1(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        private void loadComboBoxVehicleName(DataTable data, String selector)
        {
            ComboBoxVehicleName.Text = "";
            ComboBoxVehicleName.Items.Clear();
            try
            {
                try
                {
                    DataRow[] VehicleNames = data.Select("VehicleType = '" + selector + "' AND VehicleName <> ''");
                    foreach (DataRow row in VehicleNames)
                    {
                        ComboBoxVehicleName.Items.Add(row["VehicleName"].ToString().Trim());
                    }
                    if (ComboBoxVehicleName.Items.Count > 0)
                    {
                        ComboBoxVehicleName.Text = ComboBoxVehicleName.Items[0].ToString().Trim();
                    }
                }
                catch (Exception ex)
                {
                    string output = ex.Message + " loadComboBoxVehicleName"; MessageBox.Show(output);
                }
            }
            catch (Exception ex)
            {
                string output = ex.Message + " loadComboBoxVehicleName"; MessageBox.Show(output);
            }
        }

        private void ComboBoxVehicleModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            loadComboBoxVehicleName(vehicledata, ComboBoxVehicleModel.Text);
        }

        private void ComboBoxVehicleName_Leave(object sender, EventArgs e)
        {
            if (!ComboBoxVehicleName.Items.Contains(ComboBoxVehicleName.Text.Trim()))
            {
                DialogResult result = MessageBox.Show("Selected Name does not exist for the model "+ComboBoxVehicleModel.Text+", Do you want to add it?", "Delete Confirmation", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    string newVehicleType = ComboBoxVehicleName.Text.Trim();
                    try
                    {
                        ClassDatabaseConnection DatabaseConnectObj = new ClassDatabaseConnection();
                        try
                        {
                            DatabaseConnectObj.SqlQuery("INSERT INTO Vehicle (VehicleType, VehicleName) VALUES ('"+ComboBoxVehicleModel.Text+"','"+ComboBoxVehicleName.Text+"')");
                            DatabaseConnectObj.ExecutNonQuery();
                        }
                        catch (Exception ex)
                        {
                            string output = ex.Message + " ComboBoxVehicleName_Leave"; MessageBox.Show(output);
                        }
                        finally
                        {
                            DatabaseConnectObj.DatabaseConnectionClose();
                        }
                    }
                    catch (Exception ex)
                    {
                        string output = ex.Message + " ComboBoxVehicleName_Leave"; MessageBox.Show(output);
                    }
                    ComboBoxVehicleName.Items.Add(ComboBoxVehicleName.Text);
                }
                else
                {
                    ComboBoxVehicleName.Text = "";
                }
            }
        }

        private void ComboBoxItemName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboBoxItemName.Items.Contains(ComboBoxItemName.Text.ToString()))
            {
                try
                {
                    DataRow[] vehicleRowData = vehicledata.Select("VehicleType='" + ComboBoxVehicleModel.Text.Trim() + "' AND VehicleName='" + ComboBoxVehicleName.Text.Trim() + "'");
                    string selectedVehicleId = vehicleRowData[0]["Id"].ToString().Trim();
                    DataRow[] itemRowData = itemsData.Select("VehicleId = '" + selectedVehicleId + "' AND ItemName = '"+ComboBoxItemName.Text.Trim()+"'");
                    NumericUnitPrice.Value = Convert.ToDecimal(itemRowData[0]["ItemPrice"].ToString().Trim());
                }
                catch (Exception ex)
                {
                   string output = ex.Message+" ComboBoxItemName_SelectedIndexChanged"; MessageBox.Show(output);
                }
            }
        }

        private void ComboBoxItemName_Enter(object sender, EventArgs e)
        {
            ComboBoxItemName.Items.Clear();
            loadComboBoxItemName(customerData, vehicledata, itemsData);
        }

        private void ComboBoxVehicleName_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxItemName.Items.Clear();
            loadComboBoxItemName(customerData, vehicledata, itemsData);
        }

        private void ComboBoxItemName_Leave(object sender, EventArgs e)
        {
            if(!ComboBoxItemName.Items.Contains(ComboBoxItemName.Text))
            {
                DialogResult result = MessageBox.Show("the item does not exist, would you like to add a new item?", "Confirmation", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    NewService NewServiceObj = new NewService(ComboBoxVehicleModel.Text.Trim(), ComboBoxVehicleName.Text.Trim(),ComboBoxItemName.Text.Trim());
                    NewServiceObj.ShowDialog();
                    getItemsData();
                    ComboBoxItemName.Text = "";
                    ComboBoxItemName.Focus();
                }
                ComboBoxItemName.Text = "";
            }
        }

        private void ButtonAddLabour_Click(object sender, EventArgs e)
        {
            string labourId = laboursdata.Select("LabourName = '" + ComboBoxLabourName.Text.Trim() + "'")[0]["LabourId"].ToString().Trim();

            if (ComboBoxLabourName.Items.Contains(ComboBoxLabourName.Text.Trim()))
            {
                if (EditFormFlag == true)
                {

                }
                else
                {
                    //calculate total without taxn
                    decimal labourTotal = (Convert.ToDecimal(NumericLabourCharge.Value));
                    //add tax
                    labourTotal = labourTotal + (labourTotal * (Convert.ToDecimal(ComboBoxLabourTax.Text) / 100));
                    //add to data grid
                    DataGridLabour.Rows.Add(ComboBoxLabourName.Text.ToString(), NumericLabourCharge.Value.ToString(), ComboBoxLabourTax.Text, labourTotal.ToString(), labourId);
                    //update sub total
                    TextBoxSubTotal.Text = (Convert.ToInt32(TextBoxSubTotal.Text) + (NumericLabourCharge.Value)).ToString();
                    //update total
                    TextBoxTotal.Text = (Convert.ToDecimal(TextBoxTotal.Text) + NumericQuantity.Value * (NumericLabourCharge.Value + (NumericLabourCharge.Value * (Convert.ToDecimal(ComboBoxLabourTax.Text) / 100)))).ToString();
                }
                ComboBoxLabourName.Focus();
                ButtonSave.Enabled = true;
            }
            else
            {
                MessageBox.Show("Please add the new labour before adding this to the Estimate");
                ComboBoxLabourName.Focus();
            }
        }

        private void DataGridLabour_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            try
            {
                Decimal subtotal = Convert.ToInt32(e.Row.Cells["LabourCharge"].Value);
                Decimal total = Convert.ToDecimal(e.Row.Cells["LabourTotal"].Value);
                TextBoxSubTotal.Text = (Convert.ToInt32(TextBoxSubTotal.Text) - subtotal).ToString();
                TextBoxTotal.Text = (Convert.ToDecimal(TextBoxTotal.Text) - total).ToString();
            }
            catch (Exception ex)
            {
                string output = ex.Message + " dataGridView1_UserDeletingRow"; MessageBox.Show(output);
            }
        }

        private void DataGridLabour_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            Temp = Convert.ToDecimal(DataGridLabour.Rows[e.RowIndex].Cells[e.ColumnIndex].Value);
        }

        private void DataGridLabour_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string columnName = DataGridLabour.Columns[e.ColumnIndex].Name.ToString().Trim();

            if (FormLoadFlag == true && DataGridLabour.Rows[e.RowIndex].Cells[columnName].Value != null)
            {
                string datagridtax = DataGridLabour.Rows[e.RowIndex].Cells[columnName].Value.ToString();
                string datagridunitprice = DataGridLabour.Rows[e.RowIndex].Cells["UnitPrice"].Value.ToString(); ;
                if (datagridtax.EndsWith("q"))
                {
                    DataGridLabour.Rows[e.RowIndex].Cells["UnitPrice"].Value = Math.Round((Convert.ToDecimal(datagridunitprice)) / ((Convert.ToDecimal(datagridtax.Split('q')[0]) / 100) + 1), 2);
                    DataGridLabour.Rows[e.RowIndex].Cells[columnName].Value = datagridtax.Split('q')[0];
                }
                if (Convert.ToDecimal(DataGridLabour.Rows[e.RowIndex].Cells[e.ColumnIndex].Value) < Temp)
                {
                    DialogResult result = MessageBox.Show("Do you want to save changes?", "Confirmation", MessageBoxButtons.YesNo);
                    if (result == DialogResult.No)
                    {
                        DataGridLabour.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Temp;
                    }
                }
                decimal LabourTotal = (Convert.ToDecimal(DataGridLabour.Rows[e.RowIndex].Cells["LabourCharge"].Value));
                LabourTotal = LabourTotal + (LabourTotal * (Convert.ToDecimal(DataGridLabour.Rows[e.RowIndex].Cells["LabourTax"].Value) / 100));
                DataGridLabour.Rows[e.RowIndex].Cells["LabourTotal"].Value = LabourTotal;
                loadTotalAndSubTotal();
            }
            else if (FormLoadFlag == true && DataGridLabour.Rows[e.RowIndex].Cells[columnName].Value == null)
            {
                DataGridLabour.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = Temp;
            }
        }

        private void ComboBoxLabourName_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(ComboBoxLabourName.Items.Contains(ComboBoxLabourName.Text))
            {
                try
                {
                    DataRow labourRow = laboursdata.Select("LabourName = '" + ComboBoxLabourName.Text.Trim() + "'")[0];
                    NumericLabourCharge.Value = Convert.ToDecimal(labourRow["LabourPrice"].ToString().Trim());
                }
                catch (Exception ex)
                {
                    string output = ex.Message + " ComboBoxItemName_SelectedIndexChanged"; MessageBox.Show(output);
                }
            }
        }

        private void ComboBoxLabourName_Enter(object sender, EventArgs e)
        {
            ComboBoxLabourName.Items.Clear();
            loadComboBoxLabourName(laboursdata);
        }

        private void ComboBoxLabourName_Leave(object sender, EventArgs e)
        {
            if (!ComboBoxLabourName.Items.Contains(ComboBoxLabourName.Text))
            {
                DialogResult result = MessageBox.Show("the Labour does not exist, would you like to add a new Labour?", "Confirmation", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    NewLabour NewLabourObj = new NewLabour(ComboBoxLabourName.Text.Trim());
                    NewLabourObj.ShowDialog();
                    getLaboursData();
                    ComboBoxLabourName.Text = "";
                    ComboBoxLabourName.Focus();
                }
                ComboBoxLabourName.Text = "";
            }
        }

        private void NewEstimate_Load(object sender, EventArgs e)
        {
            DateTimePickerIssued.Value = DateTime.Today;
            getCustomerData();
            LoadCusCombobox();
            getVehicleData();
            loadVehiclefield();
            getItemsData();
            getLaboursData();
            try
            {
                ClassDatabaseConnection DatabaseConnectObj = new ClassDatabaseConnection();
                try
                {
                    DataTable dt = new DataTable();
                    DatabaseConnectObj.SqlQuery("SELECT MAX(EstimateNo) AS Estimateno FROM Estimate");
                    dt = DatabaseConnectObj.ExecuteQuery();
                    foreach (DataRow row in dt.Rows)
                    {
                        NumericEstimateNo.Text = (Convert.ToInt32(row["Estimateno"].ToString().Trim()) + 1).ToString();
                    }
                }
                catch (Exception ex)
                {
                    string output = ex.Message + " NewEstimate_Load"; MessageBox.Show(output);
                }
                finally
                {
                    DatabaseConnectObj.DatabaseConnectionClose();
                }
            }
            catch (Exception ex)
            {
                string output = ex.Message + " NewEstimate_Load"; MessageBox.Show(output);
            }
            if (EditFormFlag == true)
            {
                Decimal TempTotal = 0;
                Decimal TempSubTotal = 0;
                foreach (DataGridViewRow row in dataGridItems.Rows)
                {
                    decimal total = (Convert.ToDecimal(row.Cells["Quantity"].Value) * Convert.ToDecimal(row.Cells["UnitPrice"].Value));
                    TempSubTotal += total;
                    total = total + (total * (Convert.ToDecimal(row.Cells["Tax"].Value) / 100));
                    row.Cells["Total"].Value = total;
                    TempTotal += total;
                }
                TextBoxTotal.Text = TempTotal.ToString();
                TextBoxSubTotal.Text = TempSubTotal.ToString();
            }
        }

        private void ComboboxItemTax_Leave(object sender, EventArgs e)
        {
            if (ComboboxItemTax.Text.EndsWith("q"))
            {
                NumericUnitPrice.Value = (NumericUnitPrice.Value) / ((Convert.ToDecimal(ComboboxItemTax.Text.Split('q')[0]) / 100) + 1);
                ComboboxItemTax.Text = ComboboxItemTax.Text.Split('q')[0];
            }
        }
    }
}
