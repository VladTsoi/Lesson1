using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace Zadanie
{
    public partial class Form1 : Form
    {
        SqlConnection conn = MSSQLServerUtils.GetDBConnection();

        public Form1()
        {
            InitializeComponent();
            DataBase();
        }

        public void DataBase()
        {
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.AllowUserToAddRows = false;
            conn.Open();
            SqlDataAdapter adapter = new SqlDataAdapter("SELECT c.Id, c.Name AS 'Название', c.Code AS 'Код страны', " +
                "ct.Name AS 'Столица', r.Name AS 'Регион', c.Area AS 'Площадь(кв.км)', c.Population AS 'Население' FROM Country c" +
                " JOIN Region r ON r.Id = c.RegionId JOIN City ct ON ct.Id = c.CityId", conn);
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            dataGridView1.DataSource = ds.Tables[0];
            dataGridView1.ReadOnly = true;
            conn.Close();
        }

        private async void search_Click(object sender, EventArgs e)
        {
            if (textBoxSearch.Text.Length <= 0)
            {
                MessageBox.Show("Пожалуйста введите страну.");
            }
            else
            {
                try
                {
                    string countrystring = "https://restcountries.eu/rest/v2/name/" + textBoxSearch.Text;
                    WebRequest request = WebRequest.Create(countrystring);
                    request.Method = "GET";

                    request.ContentType = "application/json";

                    Country country;
                    WebResponse response = await request.GetResponseAsync();

                    string str = string.Empty;

                    using (Stream s = response.GetResponseStream())
                    {
                        using (StreamReader read = new StreamReader(s))
                        {
                            str = await read.ReadToEndAsync();
                        }
                    }
                    response.Close();

                    country = JsonConvert.DeserializeObject<Country>(str.Substring(1, str.Length - 2));
                    label_area.Text = country.area.ToString() + " кв.км";
                    label_cap.Text = country.capital;
                    label_code.Text = country.numericCode.ToString();
                    label_country.Text = country.name;
                    label_pop.Text = country.population.ToString() + " чел.";
                    label_reg.Text = country.region;
                    DialogResult vibor = MessageBox.Show("Хотите сохранить информацию в базу данных?", "Да или нет", MessageBoxButtons.YesNo);
                    if (vibor == DialogResult.Yes)
                    {  
                        
                        try
                        {
                            conn.Open();
                            SqlDataAdapter adreg = new SqlDataAdapter("SELECT Name FROM Region WHERE Name = '" + label_reg.Text + "'", conn);
                            SqlDataAdapter adcity = new SqlDataAdapter("SELECT Name FROM City WHERE Name = '" + label_cap.Text + "'", conn);
                            SqlDataAdapter adcountry = new SqlDataAdapter("SELECT Name FROM Country WHERE Code = '" + label_code.Text + "'", conn);
                            DataTable dtreg = new DataTable();
                            DataTable dtcity = new DataTable();
                            DataTable dtcountry = new DataTable();
                            adreg.Fill(dtreg);
                            adcity.Fill(dtcity);
                            adcountry.Fill(dtcountry);
                            int resultreg = dtreg.Rows.Count;
                            int resultcity = dtcity.Rows.Count;
                            int resultcountry = dtcountry.Rows.Count;
                            if (resultreg == 0)
                            {
                                using (SqlCommand cmd = new SqlCommand("INSERT INTO Region (Name) VALUES(@Name)", conn))
                                {
                                    cmd.Parameters.AddWithValue("Name", label_reg.Text);
                                    int result = cmd.ExecuteNonQuery();
                                    if (result > 0)
                                    {
                                        MessageBox.Show("Данные в таблице Region сохранены.");
                                    }
                                }
                                
                            }

                            if (resultcity == 0)
                            {
                                using (SqlCommand cmd = new SqlCommand("INSERT INTO City (Name) VALUES(@Name)", conn))
                                {
                                    cmd.Parameters.AddWithValue("Name", label_cap.Text);
                                    int result = cmd.ExecuteNonQuery();
                                    if (result > 0)
                                    {
                                        MessageBox.Show("Данные в таблице City сохранены.");
                                    }
                                }
                            }
                            
                            if (resultcountry == 0)
                            {
                                using (SqlCommand cmd = new SqlCommand("INSERT INTO Country (Name, Code, Area, Population, CityId, RegionId) " +
                                    "VALUES(@Name, @Code, @Area, @Population, (SELECT Id FROM City WHERE Name = '" + label_cap.Text + "'), " +
                                    "(SELECT Id FROM Region WHERE Name = '" + label_reg.Text + "'))", conn))
                                {
                                    cmd.Parameters.AddWithValue("Name", label_country.Text);
                                    cmd.Parameters.AddWithValue("Code", label_code.Text);
                                    cmd.Parameters.AddWithValue("Area", country.area.ToString());
                                    cmd.Parameters.AddWithValue("Population", country.population.ToString());
                                    int result = cmd.ExecuteNonQuery();
                                    if (result > 0)
                                    {
                                        MessageBox.Show("Данные в таблице Country сохранены.");
                                    }

                                }
                            }
                            else
                            {
                                using (SqlCommand cmd = new SqlCommand("UPDATE c SET c.Name = '" + label_country.Text + "', c.Area = '" 
                                    + country.area.ToString() + "', c.Population = '" + country.population.ToString() + "', c.CityId = ct.Id, c.RegionId = r.Id" + 
                                    " FROM Country c, Region r, City ct WHERE c.Code = '" + label_code.Text + "'", conn))
                                {
                                    cmd.Parameters.AddWithValue("Name", label_country.Text);
                                    cmd.Parameters.AddWithValue("Area", country.area.ToString());
                                    cmd.Parameters.AddWithValue("Population", country.population.ToString());
                                    int result = cmd.ExecuteNonQuery();
                                    if (result > 0)
                                    {
                                        MessageBox.Show("Данные обновлены!");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Ошибка: " + ex.Message);
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                }
                catch
                {
                    MessageBox.Show("Страна " + textBoxSearch.Text + " не найдена");
                }
            }
        }
    }
}
