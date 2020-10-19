using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Zadanie
{
    class SaveData
    {
        string connString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
        public Country _country;
        SqlConnection conn;
        public void GetSaveCity()
        {
            conn = new SqlConnection(connString);
            conn.Open();
            using (SqlDataAdapter adcity = new SqlDataAdapter("SELECT Name FROM City WHERE Name = '" + _country.capital + "'", conn))
            {
                DataTable dtcity = new DataTable();
                adcity.Fill(dtcity);
                if (dtcity.Rows.Count == 0)
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO City (Name) VALUES(@Name)", conn))
                    {
                        cmd.Parameters.AddWithValue("Name", _country.capital);
                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            MessageBox.Show("Данные в таблице City сохранены.");
                        }
                    }
                }
            }
        }

        public void GetSaveRegion()
        {
            conn = new SqlConnection(connString);
            conn.Open();
            using (SqlDataAdapter adreg = new SqlDataAdapter("SELECT Name FROM Region WHERE Name = '" + _country.region + "'", conn))
            {
                DataTable dtreg = new DataTable();
                adreg.Fill(dtreg);
                if (dtreg.Rows.Count == 0)
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Region (Name) VALUES(@Name)", conn))
                    {
                        cmd.Parameters.AddWithValue("Name", _country.region);
                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            MessageBox.Show("Данные в таблице Region сохранены.");
                        }
                    }
                }
            }

        }
        public void GetSaveCountry()
        {
            conn = new SqlConnection(connString);
            conn.Open();
            using (SqlDataAdapter adcountry = new SqlDataAdapter("SELECT Name FROM Country WHERE Code = '" + _country.numericCode + "'", conn))
            {
                DataTable dtcountry = new DataTable();
                adcountry.Fill(dtcountry);
                if (dtcountry.Rows.Count == 0)
                {
                    using (SqlCommand cmd = new SqlCommand("INSERT INTO Country (Name, Code, Area, Population, CityId, RegionId) " +
                        "VALUES(@Name, @Code, @Area, @Population, (SELECT Id FROM City WHERE Name = '" + _country.capital + "'), " +
                        "(SELECT Id FROM Region WHERE Name = '" + _country.region + "'))", conn))
                    {
                        cmd.Parameters.AddWithValue("Name", _country.name);
                        cmd.Parameters.AddWithValue("Code", _country.numericCode);
                        cmd.Parameters.AddWithValue("Area", _country.area.ToString());
                        cmd.Parameters.AddWithValue("Population", _country.population.ToString());
                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            MessageBox.Show("Данные в таблице Country сохранены.");
                        }

                    }
                }
                else
                {
                    using (SqlCommand cmd = new SqlCommand("UPDATE c SET c.Name = '" + _country.name + "', c.Area = '"
                        + _country.area.ToString() + "', c.Population = '" + _country.population.ToString() + "', c.CityId = ct.Id, c.RegionId = r.Id" +
                        " FROM Country c, Region r, City ct WHERE c.Code = '" + _country.numericCode + "'", conn))
                    {
                        cmd.Parameters.AddWithValue("Name", _country.name);
                        cmd.Parameters.AddWithValue("Area", _country.area.ToString());
                        cmd.Parameters.AddWithValue("Population", _country.population.ToString());
                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            MessageBox.Show("Данные обновлены!");
                        }
                    }
                }
            }    
        }
    }
}
