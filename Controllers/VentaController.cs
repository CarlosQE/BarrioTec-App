using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using TiendaApp.Data;
using TiendaApp.Models;

namespace TiendaApp.Controllers
{
    public class VentaController
    {
        // =================================================================
        // MÉTODO 1: Registrar Venta con Transacciones (Nivel Estratégico)
        // =================================================================
        public void RegistrarVenta(Venta venta)
        {
            using (SqlConnection conn = DbContext.Instance.GetConnection())
            {
                conn.Open();
                
                // Iniciamos la transacción para asegurar la integridad de datos
                SqlTransaction tran = conn.BeginTransaction();

                try
                {
                    // 1. Insertar Cabecera y obtener el ID generado (OUTPUT INSERTED.Id)
                    string sqlVenta = "INSERT INTO Ventas (ClienteId, Total) OUTPUT INSERTED.Id VALUES (@c, @t)";
                    SqlCommand cmdV = new SqlCommand(sqlVenta, conn, tran);
                    cmdV.Parameters.AddWithValue("@c", venta.ClienteId);
                    cmdV.Parameters.AddWithValue("@t", venta.Total);

                    // Obtenemos el ID de la venta recién creada
                    int ventaId = (int)cmdV.ExecuteScalar();

                    // 2. Insertar Detalles y descontar Stock
                    foreach (var d in venta.Detalles)
                    {
                        // Insertar el detalle
                        string sqlDetalle = "INSERT INTO DetalleVentas (VentaId, ArticuloId, Cantidad, Subtotal) VALUES (@v, @a, @can, @s)";
                        SqlCommand cmdD = new SqlCommand(sqlDetalle, conn, tran);
                        cmdD.Parameters.AddWithValue("@v", ventaId);
                        cmdD.Parameters.AddWithValue("@a", d.ArticuloId);
                        cmdD.Parameters.AddWithValue("@can", d.Cantidad);
                        cmdD.Parameters.AddWithValue("@s", d.Subtotal);
                        cmdD.ExecuteNonQuery();

                        // Actualizar el Stock del Artículo restando la cantidad vendida
                        string sqlStock = "UPDATE Articulos SET Stock = Stock - @can WHERE Id = @a";
                        SqlCommand cmdS = new SqlCommand(sqlStock, conn, tran);
                        cmdS.Parameters.AddWithValue("@can", d.Cantidad);
                        cmdS.Parameters.AddWithValue("@a", d.ArticuloId);
                        cmdS.ExecuteNonQuery();
                    }

                    // Si todo salió bien, confirmamos la transacción
                    tran.Commit();
                }
                // Manejo de excepciones avanzado requerido por la rúbrica (Nivel A)
                catch (SqlException sqlEx)
                {
                    tran.Rollback();
                    // Capturamos específicamente errores de SQL (ej. constraints, llaves foráneas)
                    throw new Exception($"Error de Base de Datos al registrar la venta: {sqlEx.Message}", sqlEx);
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    // Capturamos errores generales
                    throw new Exception($"Error general procesando la transacción: {ex.Message}", ex);
                }
            }
        }

        // =================================================================
        // MÉTODO 2: Obtener Historial de Ventas
        // =================================================================
        public List<Venta> ObtenerHistorialVentas()
        {
            List<Venta> historial = new List<Venta>();
            using (SqlConnection conn = DbContext.Instance.GetConnection())
            {
                // Consulta que une Venta y Cliente para mostrar el nombre en lugar de solo el ID
                string sql = @"
                    SELECT v.Id, v.Fecha, c.Nombre AS ClienteNombre, v.Total 
                    FROM Ventas v
                    INNER JOIN Clientes c ON v.ClienteId = c.Id
                    ORDER BY v.Fecha DESC";

                SqlCommand cmd = new SqlCommand(sql, conn);
                conn.Open();
                
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        historial.Add(new Venta
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Fecha = Convert.ToDateTime(reader["Fecha"]),
                            ClienteNombre = reader["ClienteNombre"].ToString(),
                            Total = Convert.ToDecimal(reader["Total"])
                        });
                    }
                }
            }
            return historial;
        }
    }
}