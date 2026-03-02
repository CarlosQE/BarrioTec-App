using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TiendaApp.Data;
using TiendaApp.Models;

namespace TiendaApp.Controllers
{
    public class ArticuloController
    {
        // Método para listar o buscar artículos
        public List<Articulo> BuscarArticulos(string filtro = "")
        {
            List<Articulo> lista = new List<Articulo>();
            using (SqlConnection conn = DbContext.Instance.GetConnection())
            {
                string sql = "SELECT Id, Nombre, Precio, Stock FROM Articulos WHERE Nombre LIKE @filtro";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@filtro", "%" + filtro + "%");

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new Articulo
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Nombre = reader["Nombre"].ToString(),
                            Precio = Convert.ToDecimal(reader["Precio"]),
                            Stock = Convert.ToInt32(reader["Stock"])
                        });
                    }
                }
            }
            return lista;
        }

        // Método para agregar un nuevo artículo
        public void AgregarArticulo(Articulo articulo)
        {
            using (SqlConnection conn = DbContext.Instance.GetConnection())
            {
                string sql = "INSERT INTO Articulos (Nombre, Precio, Stock) VALUES (@nombre, @precio, @stock)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nombre", articulo.Nombre);
                cmd.Parameters.AddWithValue("@precio", articulo.Precio);
                cmd.Parameters.AddWithValue("@stock", articulo.Stock);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}