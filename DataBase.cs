using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SushiMix
{
    internal class DataBase
    {
        private string path = "datasource=localhost;port=3306;database=sysi_mix;username=root;password=toor";
        public List<Product> pizza30sm = new List<Product>();
        public List <Product> setu = new List<Product>();
        public List<Product> rolu = new List<Product>();
        public List<Product> maki = new List<Product>();
        public List<Product> salat = new List<Product>();
        public List <Product> sashimi = new List<Product>();
        public List<Product> soki = new List<Product>();
        public List<Product> susi = new List<Product>();
        public List<Product> voda = new List<Product>();
        public List<Product> pivo = new List<Product>();
        public List<Product> pizza45sm = new List<Product>();
        public List<Product> allProducts = new List<Product>();
        public DataBase ()
        {
            string Request = "SELECT id, name, description, price, img, category FROM sysi_mix.products";
            MySqlConnection connection = new MySqlConnection(path);
            connection.Open();
            MySqlCommand command = new MySqlCommand(Request, connection);
            MySqlDataReader reader = command.ExecuteReader();
          
            while (reader.Read())
            {
                allProducts.Add(new Product() { id=Convert.ToInt32(reader[0]), name=reader[1].ToString(), description=reader[2].ToString(),price=Convert.ToInt32(reader[3]),img=reader[4].ToString(), category=reader[5].ToString()});
            }
            reader.Close();




            setu = (from a in allProducts where a.category == "Сети" select a).ToList();
            rolu = (from a in allProducts where a.category == "Роли" select a).ToList<Product>();
            sashimi = (from a in allProducts where a.category == "Сашими" select a).ToList<Product>();
            salat = (from a in allProducts where a.category == "Салати та супи" select a).ToList<Product>();
            maki = (from a in allProducts where a.category == "Маки" select a).ToList<Product>();
            susi = (from a in allProducts where a.category == "Суши" select a).ToList<Product>();

            pizza45sm = (from a in allProducts where a.category == "Пицца_45см" select a).ToList<Product>();
            pivo = (from a in allProducts where a.category == "Пиво" select a).ToList<Product>();
            voda = (from a in allProducts where a.category == "Вода" select a).ToList<Product>();
            soki = (from a in allProducts where a.category == "Соки" select a).ToList<Product>();
            pizza30sm = (from a in allProducts where a.category == "Пицца_30см" select a).ToList<Product>();
           
        }
        public void Insert(long tg_id, string username, string first_name, string last_name, int id_tovar)
        {
            //INSERT INTO `sysi_mix`.`korzina` (`telegram_id`, `username`, `firstname`, `lastname`, `idTovar`, `countTovar`) VALUES('1', '2', '3', '4', '5', '6');
            string Request = "INSERT INTO `sysi_mix`.`korzina` (`telegram_id`, `username`, `firstname`, `lastname`, `idTovar`) VALUES ('"+ tg_id + "', '" + username + "', '" + first_name + "', '" + last_name + "', '" + id_tovar + "');";
            MySqlConnection connection = new MySqlConnection(path);
            connection.Open();
            MySqlCommand command = new MySqlCommand(Request, connection);
            command.ExecuteScalarAsync();
        }


        public void Delete(long tg_id, int id_tovar)

        {
            //INSERT INTO `sysi_mix`.`korzina` (`telegram_id`, `username`, `firstname`, `lastname`, `idTovar`, `countTovar`) VALUES('1', '2', '3', '4', '5', '6');
            string Request = "DELETE FROM `sysi_mix`.`korzina` WHERE idTovar = '"+id_tovar+ "' AND telegram_id = '"+tg_id+"' LIMIT 1;";
            MySqlConnection connection = new MySqlConnection(path);
            connection.Open();
            MySqlCommand command = new MySqlCommand(Request, connection);
            command.ExecuteScalarAsync();
        }

        public List<Product> Tovarsinkorzina(long telegram_id)
        {
            List<Product> prodycts_in_korzina = new List<Product>();
            string Request = "SELECT id, idTovar, telegram_id FROM sysi_mix.korzina WHERE telegram_id='"+telegram_id+"';";
            MySqlConnection connection = new MySqlConnection(path);
            connection.Open();
            MySqlCommand command = new MySqlCommand(Request, connection);
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                bool check = false;
                for (int i = 0; i < prodycts_in_korzina.Count; i++)
                {
                    if (prodycts_in_korzina[i].id == Convert.ToInt32(reader[1]))
                    {
                        prodycts_in_korzina[i].count++;
                        check = true; break;
                    }
                }
                if (check == false)
                {

                    prodycts_in_korzina.Add(TovarInCorzina(Convert.ToInt32(reader[1])));
                    prodycts_in_korzina[prodycts_in_korzina.Count - 1].count = 1;
                }
            }
            reader.Close();
            return prodycts_in_korzina;

        }
        public Product TovarInCorzina(int a)
        {
            string Request = "SELECT id, name, description, price, img, category FROM sysi_mix.products WHERE id = '"+ a +"' ";
            MySqlConnection connection = new MySqlConnection(path);
            connection.Open();
            MySqlCommand command = new MySqlCommand(Request, connection);
            MySqlDataReader reader = command.ExecuteReader();
            Product v = null;
            while (reader.Read())
            {
             v=new Product() { id = Convert.ToInt32(reader[0]), name = reader[1].ToString(), description = reader[2].ToString(), price = Convert.ToInt32(reader[3]), img = reader[4].ToString(), category = reader[5].ToString() };
            }
            reader.Close();
            return v;
        }


    }
}
