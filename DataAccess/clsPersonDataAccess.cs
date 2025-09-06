using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class clsPersonDataAccess
    {
        private static string Connection = clsDataAccessSettings.ConnectionString;

        private const string SelectQuery = @"SELECT PersonID, NationalNo, FirstName, SecondName, ThirdName, LastName, 
                                                     DateOfBirth, Gender, Address, Phone, Email, NationalityCountryID, ImagePath  
                                            FROM    People WHERE PersonID = @PersonID";

        private const string InsertQuery = @"INSERT INTO People (NationalNo, FirstName, SecondName, ThirdName, LastName, Gender, Address, DateOfBirth, 
                                                        Phone, Email, NationalityCountryID, ImagePath)
                                            VALUES (@NationalNo, @FirstName, @SecondName, @ThirdName, @LastName, @Gender, @Address, @DateOfBirth, 
                                                    @Phone, @Email, @NationalityCountryID, @ImagePath);
                                            SELECT SCOPE_IDENTITY();";

        private const string UpdateQuery = @"Update  People  
                        SET NationalNo = @NationalNo, 
                            FirstName = @FirstName, 
                            SecondName = @SecondName, 
                            ThirdName = @ThirdName, 
                            LastName = @LastName, 
                            Gender = @Gender, 
                            Address = @Address, 
                            DateOfBirth = @DateOfBirth, 
                            Phone = @Phone, 
                            Email = @Email, 
                            NationalityCountryID = @NationalityCountryID, 
                            ImagePath = @ImagePath
                            where PersonID = @PersonID";

        private const string AllPeopleQuery = "SELECT * FROM People";

        private const string DeletePersonQuery = @"DELETE People WHERE PersonID = @PersonID";

        private const string NationalNoExistsQuery = "SELECT Found=1 FROM People WHERE NationalNo = @NationalNo";

        private const string SelectByNationalNoQuery = @"SELECT * FROM People WHERE NationalNo = @NationalNo";

        private static string PeopleWithStringNamesQuery = @"SELECT People.PersonID, People.NationalNo, People.FirstName, People.SecondName, 
                                                                       People.ThirdName, People.LastName, 
                                                          CASE WHEN People.Gender = 0 THEN 'Male' ELSE 'Female' END AS Gender,
                                                          People.DateOfBirth, Countries.CountryName AS Nationality, 
                                                          People.Phone, People.Email  
                                                          FROM People 
                                                          INNER JOIN Countries ON People.NationalityCountryID = Countries.CountryID";

        private static T GetValueOrDefault<T>(SqlDataReader reader, string ColmName, T DefaultValue)
        {
            var ReaderValue = reader[ColmName];
            return ReaderValue != DBNull.Value ? (T)ReaderValue : DefaultValue;
        }

        public static bool GetPersonInfoByID(int ID, ref string NationalNo, ref string FirstName, ref string SecondName, ref string ThirdName,
            ref string LastName, ref DateTime DateOfBirth, ref short Gender, ref string Address, ref string Phone,
            ref string Email, ref int NationalityCountryID, ref string ImagePath)
        {

            SqlConnection SqlConnection = new SqlConnection(Connection);

            SqlCommand command = new SqlCommand(SelectQuery, SqlConnection);

            command.Parameters.AddWithValue("@PersonID", ID);

            bool isFound = false;

            try
            {
                SqlConnection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    // The record was found
                    isFound = true;

                    // NationalNo =  (string)reader["NationalNo"]; // old style reader 

                    NationalNo = GetValueOrDefault(reader, "NationalNo", string.Empty);
                    FirstName = GetValueOrDefault(reader, "FirstName", string.Empty);
                    SecondName = GetValueOrDefault(reader, "SecondName", string.Empty);
                    ThirdName = GetValueOrDefault(reader, "ThirdName", string.Empty);
                    LastName = GetValueOrDefault(reader, "LastName", string.Empty);
                    DateOfBirth = GetValueOrDefault(reader, "DateOfBirth", DateTime.MinValue);
                    Gender = GetValueOrDefault(reader, "Gender", (short)-1);
                    Address = GetValueOrDefault(reader, "Address", string.Empty);
                    Phone = GetValueOrDefault(reader, "Phone", string.Empty);
                    Email = GetValueOrDefault(reader, "Email", string.Empty);
                    NationalityCountryID = GetValueOrDefault(reader, "NationalityCountryID", -1);
                    ImagePath = GetValueOrDefault(reader, "ImagePath", string.Empty);
                }
                else
                {
                    // The record was not found
                    isFound = false;
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                isFound = false;
            }
            finally
            {
                SqlConnection.Close();
            }

            return isFound;
        }

        public static int AddNewPerson(string NationalNo, string FirstName, string SecondName, string ThirdName,
             string LastName, short Gender, string Address, DateTime DateOfBirth, string Phone,
             string Email, int NationalityCountryID, string ImagePath)
        {
            //this function will return the new Person id if succeeded and -1 if not.

            if (string.IsNullOrEmpty(NationalNo))
            {
                throw new ArgumentException("NationalNo is required and cannot be empty.");
            }
            if (string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(SecondName) || string.IsNullOrEmpty(LastName))
            {
                throw new ArgumentException("First or Second or Last Name is required and cannot be empty.");
            }
            if (DateOfBirth > DateTime.Now)
            {
                throw new ArgumentException("Date Of Birth cannot be in the future.");
            }
            if (Gender != 0 && Gender != 1)
            {
                throw new ArgumentException("Invalid gender value. Please select Male or Female.");
            }
            if (string.IsNullOrEmpty(Address))
            {
                throw new ArgumentException("Address is required and cannot be empty.");
            }
            if (string.IsNullOrEmpty(Phone))
            {
                throw new ArgumentException("Phone is required and cannot be empty.");
            }
            if (NationalityCountryID < 0)
            {
                throw new ArgumentException("NationalityCountryID must be a valid positive integer.");
            }

            SqlConnection SqlConnection = new SqlConnection(Connection);

            SqlCommand command = new SqlCommand(InsertQuery, SqlConnection);

            command.Parameters.AddWithValue("@NationalNo", NationalNo);
            command.Parameters.AddWithValue("@FirstName", FirstName);
            command.Parameters.AddWithValue("@SecondName", SecondName);
            command.Parameters.AddWithValue("@ThirdName", string.IsNullOrWhiteSpace(ThirdName) ? null : ThirdName);
            command.Parameters.AddWithValue("@LastName", LastName);
            command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
            command.Parameters.AddWithValue("@Gender", Gender);
            command.Parameters.AddWithValue("@Address", Address);
            command.Parameters.AddWithValue("@Phone", Phone);
            command.Parameters.AddWithValue("@Email", string.IsNullOrWhiteSpace(Email) ? "" : Email);
            command.Parameters.AddWithValue("@NationalityCountryID", NationalityCountryID);
            command.Parameters.AddWithValue("@ImagePath", string.IsNullOrWhiteSpace(ImagePath) ? "" : ImagePath);

            try
            {
                SqlConnection.Open();

                object result = command.ExecuteScalar();
                //SqlConnection.Close();

                if (result != null && int.TryParse(result.ToString(), out int insertedID))
                {
                    return insertedID;
                }
            }
            catch (Exception)
            {
                //Console.WriteLine("Error: " + ex.Message); 
            }
            finally
            {
                SqlConnection.Close();
            }

            return -1;
        }

        public static bool UpdatePerson(int ID, string NationalNo, string FirstName, string SecondName, string ThirdName,
             string LastName, short Gender, string Address, DateTime DateOfBirth, string Phone,
             string Email, int NationalityCountryID, string ImagePath)
        {

            int rowsAffected = 0;

            SqlConnection SqlConnection = new SqlConnection(Connection);
            SqlCommand command = new SqlCommand(UpdateQuery, SqlConnection);

            command.Parameters.AddWithValue("@NationalNo", NationalNo);
            command.Parameters.AddWithValue("@FirstName", FirstName);
            command.Parameters.AddWithValue("@SecondName", SecondName);
            command.Parameters.AddWithValue("@ThirdName", ThirdName);
            command.Parameters.AddWithValue("@LastName", LastName);
            command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
            command.Parameters.AddWithValue("@Gender", Gender);
            command.Parameters.AddWithValue("@Address", Address);
            command.Parameters.AddWithValue("@Phone", Phone);
            command.Parameters.AddWithValue("@Email", Email);
            command.Parameters.AddWithValue("@NationalityCountryID", NationalityCountryID);
            command.Parameters.AddWithValue("@ImagePath", ImagePath);

            try
            {
                SqlConnection.Open();
                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                //Console.WriteLine("Error: " + ex.Message);
                return false;
            }
            finally
            {
                SqlConnection.Close();
            }

            return (rowsAffected > 0);
        }

        public static DataTable GetAllPeople()
        {
            DataTable dt = new DataTable();

            SqlConnection SqlConnection = new SqlConnection(Connection);
            SqlCommand command = new SqlCommand(AllPeopleQuery, SqlConnection);

            try
            {
                SqlConnection.Open();

                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    dt.Load(reader);
                }

                reader.Close();
            }
            catch (Exception)
            {
                // error log here
            }
            finally
            {
                SqlConnection.Close();
            }

            return dt;
        }

        public static bool DeletePerson(int PersonID)
        {
            int rowsAffected = 0;

            SqlConnection SqlConnection = new SqlConnection(Connection);
            SqlCommand command = new SqlCommand(DeletePersonQuery, SqlConnection);

            command.Parameters.AddWithValue("@PersonID", PersonID);

            try
            {
                SqlConnection.Open();

                rowsAffected = command.ExecuteNonQuery();
            }
            catch (Exception)
            {
                // Console.WriteLine("Error: " + ex.Message);
            }
            finally
            {
                SqlConnection.Close();
            }

            return (rowsAffected > 0);
        }

        // .......................
        public static bool IsNationalNumExist(string NationalNo)
        {
            bool isFound = false;

            SqlConnection SqlConnection = new SqlConnection(Connection);
            SqlCommand command = new SqlCommand(NationalNoExistsQuery, SqlConnection);

            command.Parameters.AddWithValue("@NationalNo", NationalNo);

            try
            {
                SqlConnection.Open();
                SqlDataReader reader = command.ExecuteReader();

                isFound = reader.HasRows;

                reader.Close();
            }
            catch (Exception)
            {
                //Console.WriteLine("Error: " + ex.Message);
                isFound = false;
            }
            finally
            {
                SqlConnection.Close();
            }

            return isFound;
        }

        public static bool GetPersonInfoByNationalNo(int ID, ref string NationalNo, ref string FirstName, ref string SecondName, ref string ThirdName,
             ref string LastName, ref short Gender, ref string Address, ref DateTime DateOfBirth, ref string Phone, ref string Email, ref int NationalityCountryID,
              ref string ImagePath)
        {

            bool IsFound = false;

            SqlConnection SqlConnection = new SqlConnection(Connection);

            SqlCommand Command = new SqlCommand(SelectByNationalNoQuery, SqlConnection);
            Command.Parameters.AddWithValue("@NationalNo", NationalNo);

            try
            {
                SqlConnection.Open();
                SqlDataReader DataReader = Command.ExecuteReader();

                if (DataReader.Read())
                {
                    IsFound = true;

                    NationalNo = GetValueOrDefault(DataReader, "NationalNo", string.Empty);
                    FirstName = GetValueOrDefault(DataReader, "FirstName", string.Empty);
                    SecondName = GetValueOrDefault(DataReader, "SecondName", string.Empty);
                    ThirdName = GetValueOrDefault(DataReader, "ThirdName", string.Empty);
                    DateOfBirth = GetValueOrDefault(DataReader, "DateOfBirth", DateTime.MinValue);
                    Gender = GetValueOrDefault(DataReader, "DateOfBirth", (short)-1);
                    Address = GetValueOrDefault(DataReader, "Address", string.Empty);
                    Phone = GetValueOrDefault(DataReader, "Phone", string.Empty);
                    Email = GetValueOrDefault(DataReader, "Email", string.Empty);
                    NationalityCountryID = GetValueOrDefault(DataReader, "NationalityCountryID", -1);
                    ImagePath = GetValueOrDefault(DataReader, "ImagePath", string.Empty);
                }
                else
                {
                    IsFound = false;
                }

                DataReader.Close();
            }
            catch (Exception e)
            {
                // Error Log
            }
            finally
            {
                SqlConnection.Close();
            }

            return IsFound;
        }

        public static bool IsPersonExist(object Parameter, clsSharedTypes.enPersonColSelect PersonColmSelect)
        {
            string Query = "";
            bool IsFound = false;
            int GenderValue = -1;

            switch (PersonColmSelect)
            {
                case clsSharedTypes.enPersonColSelect.ePersonID:
                    Query = @"SELECT 1 FROM People WHERE PersonID = @ParameterValue";
                    break;

                case clsSharedTypes.enPersonColSelect.eNationalNo:
                    Query = @"SELECT 1 FROM People WHERE NationalNo = @ParameterValue";
                    break;

                case clsSharedTypes.enPersonColSelect.eFirstName:
                    Query = @"SELECT 1 FROM People WHERE FirstName = @ParameterValue";
                    break;

                case clsSharedTypes.enPersonColSelect.eSecondName:
                    Query = @"SELECT 1 FROM People WHERE SecondName = @ParameterValue";
                    break;

                case clsSharedTypes.enPersonColSelect.eThirdName:
                    Query = @"SELECT 1 FROM People WHERE ThirdName = @ParameterValue";
                    break;

                case clsSharedTypes.enPersonColSelect.eLastName:
                    Query = @"SELECT 1 FROM People WHERE LastName = @ParameterValue";
                    break;

                case clsSharedTypes.enPersonColSelect.eNationality:
                    Query = @"SELECT 1 FROM People INNER JOIN Countries ON People.NationalityCountryID = Countries.CountryID 
                      WHERE Countries.CountryName = @ParameterValue";
                    break;

                case clsSharedTypes.enPersonColSelect.eGender:
                    GenderValue = (Parameter.ToString().ToLower() == "male") ? 0 : 1;
                    Query = @"SELECT 1 FROM People WHERE Gender = @ParameterValue";
                    break;

                case clsSharedTypes.enPersonColSelect.ePhone:
                    Query = @"SELECT 1 FROM People WHERE Phone = @ParameterValue";
                    break;

                case clsSharedTypes.enPersonColSelect.eEmail:
                    Query = @"SELECT 1 FROM People WHERE Email = @ParameterValue";
                    break;

                default:
                    return false; // Invalid column, return false to avoid executing a query
            }

            SqlConnection SqlConnection = new SqlConnection(Connection);

            SqlCommand Command = new SqlCommand(Query, SqlConnection);
            Command.Parameters.AddWithValue("@ParameterValue", GenderValue != -1 ? GenderValue : Parameter);

            try
            {
                SqlConnection.Open();
                object Result = Command.ExecuteScalar();
                IsFound = (Result != null && Result != DBNull.Value);
            }
            catch (Exception e)
            {
                // error log here
            }
            finally
            {
                SqlConnection.Close();
            }

            return IsFound;
        }

        public static DataTable FilterPersonBy(object ColumnValue, clsSharedTypes.enPersonColSelect PersonColmSelect)
        {

            DataTable dtAllPerson = PeopleDataWithStringNames();
            DataView dvPerson = dtAllPerson.DefaultView;

            if (ColumnValue == null)
                return dtAllPerson;

            string columnName;
            switch (PersonColmSelect)
            {
                case clsSharedTypes.enPersonColSelect.ePersonID:
                    {
                        dvPerson.RowFilter = $"PersonID = {Convert.ToInt32(ColumnValue)}";
                        return dvPerson.ToTable();
                    }
                case clsSharedTypes.enPersonColSelect.eNationalNo:
                    {
                        columnName = "NationalNo";
                        break;
                    }
                case clsSharedTypes.enPersonColSelect.eFirstName:
                    {
                        columnName = "FirstName";
                        break;
                    }
                case clsSharedTypes.enPersonColSelect.eSecondName:
                    {
                        columnName = "SecondName";
                        break;
                    }
                case clsSharedTypes.enPersonColSelect.eThirdName:
                    {
                        columnName = "ThirdName";
                        break;
                    }
                case clsSharedTypes.enPersonColSelect.eLastName:
                    {
                        columnName = "LastName";
                        break;
                    }
                case clsSharedTypes.enPersonColSelect.eNationality:
                    {
                        columnName = "Nationality";
                        dvPerson = PeopleDataWithStringNames().DefaultView;
                        break;
                    }
                case clsSharedTypes.enPersonColSelect.eGender:
                    {
                        columnName = "Gender";
                        dvPerson = PeopleDataWithStringNames().DefaultView;
                        break;
                    }
                case clsSharedTypes.enPersonColSelect.ePhone:
                    {
                        columnName = "Phone";
                        break;
                    }
                case clsSharedTypes.enPersonColSelect.eEmail:
                    {
                        columnName = "Email";
                        break;
                    }

                default:
                    return dtAllPerson;
            }

            // to prevent SQL injection
            string escapedValue = Convert.ToString(ColumnValue).Replace("'", "''");
            dvPerson.RowFilter = $"{columnName} = '{escapedValue}'";
            return dvPerson.ToTable();
        }

        public static DataTable PeopleDataWithStringNames()
        {

            DataTable dt = new DataTable();
            SqlConnection SqlConnection = new SqlConnection(Connection);
            SqlCommand Command = new SqlCommand(PeopleWithStringNamesQuery, SqlConnection);

            try
            {
                SqlConnection.Open();
                SqlDataReader DataReader = Command.ExecuteReader();

                if (DataReader.HasRows)
                {
                    dt.Load(DataReader);
                }

                DataReader.Close();
            }
            catch (Exception e)
            {
                // Error Log
            }
            finally
            {
                SqlConnection.Close();
            }

            return dt;
        }
    }
}
