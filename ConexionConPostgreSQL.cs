using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    public class CConexion
    {
        NpgsqlConnection conex = new NpgsqlConnection();
        static String servidor=
        
        
        "localhost"; //pruebas
        //"192.168.1.104";//ofi
        //"192.168.1.40";//casa
        static String bd=
        //"BD"; 
        "BDPruebas";//Pruebas Pc Eri
        static String usuario="postgres";
        static String password="admin";
        static String puerto = "5432";

        String cadenaConexion = "server=" + servidor + ";" + "port=" + puerto + ";" + "user id=" + usuario + ";" + "password=" + password + ";" + "database=" + bd + ";";

        public NpgsqlConnection establecerConexion() {

            try {
                conex.ConnectionString = cadenaConexion;
                conex.Open();
                Console.WriteLine("Se conectó correctamente a la Base de datos");
            }

            catch (NpgsqlException e) {
                Console.WriteLine("No se pudo conectar a la base de datos de PostgreSQL, error: "+ e.ToString());
            
            }

            return conex;
        }
        public void cerrarConexion(NpgsqlConnection conex ){
            try {
                conex.Close();
                Console.WriteLine("Se desconecto  correctamente a la Base de datos");
            }catch (NpgsqlException e) {
                Console.WriteLine("No se pudo desconectar a la base de datos de PostgreSQL, error: "+ e.ToString());
            
            }
        }

    }