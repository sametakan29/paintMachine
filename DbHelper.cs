using Npgsql;
using System;
using System.Collections.Generic;

namespace paintMachine
{
    public static class DbHelper
    {
        private static readonly string connString = "Host=localhost;Port=5432;Database=PaintDb;Username=postgres;Password=admin";

        // 🔌 Merkezi Bağlantı Oluşturucu
        private static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(connString);
        }

        public static List<ColorRecipe> GetRecipes()
        {
            var list = new List<ColorRecipe>();

            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT id, colorname, hexcode, servo1ml, servo2ml FROM ColorRecipes";

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ColorRecipe
                        {
                            Id = reader.GetInt32(0),
                            ColorName = reader.GetString(1),
                            HexCode = reader.GetString(2),
                            Servo1Ml = Convert.ToDouble(reader.GetDecimal(3)),
                            Servo2Ml = Convert.ToDouble(reader.GetDecimal(4))
                        });
                    }
                }
            }

            return list;
        }

        public static void AddLog(int recipeId, string colorName, double s1Used, double s2Used)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = @"INSERT INTO PaintLogs (RecipeId, ColorName, Servo1UsedMl, Servo2UsedMl) 
                                VALUES (@recipeId, @colorName, @s1Used, @s2Used)";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("recipeId", recipeId);
                    cmd.Parameters.AddWithValue("colorName", colorName);
                    cmd.Parameters.AddWithValue("s1Used", s1Used);
                    cmd.Parameters.AddWithValue("s2Used", s2Used);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static List<PaintLog> GetLogs()
        {
            var list = new List<PaintLog>();

            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT id, colorname, servo1usedml, servo2usedml, createddate FROM PaintLogs ORDER BY createddate DESC";

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new PaintLog
                        {
                            Id = reader.GetInt32(0),
                            ColorName = reader.GetString(1),
                            Servo1UsedMl = Convert.ToDouble(reader.GetDecimal(2)),
                            Servo2UsedMl = Convert.ToDouble(reader.GetDecimal(3)),
                            CreatedDate = reader.GetDateTime(4)
                        });
                    }
                }
            }

            return list;
        }

        // 🛢️ Depodaki Mevcut Miktarları Okuma
        public static (double servo1Ml, double servo2Ml) GetTankLevels()
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "SELECT servo1_ml, servo2_ml FROM tanks WHERE id = 1";

                using (var cmd = new NpgsqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        double s1 = Convert.ToDouble(reader["servo1_ml"]);
                        double s2 = Convert.ToDouble(reader["servo2_ml"]);
                        return (s1, s2);
                    }
                }
            }
            return (1000.0, 1000.0); // Varsayılan değer
        }

        // 🛢️ Depodaki Miktarları Güncelleme
        public static void UpdateTankLevels(double servo1Ml, double servo2Ml)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                string query = "UPDATE tanks SET servo1_ml = @s1, servo2_ml = @s2 WHERE id = 1";

                using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@s1", servo1Ml);
                    cmd.Parameters.AddWithValue("@s2", servo2Ml);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}