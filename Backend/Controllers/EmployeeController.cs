using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using ApiService.Models;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace ApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public EmployeeController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [HttpGet]
        public JsonResult Get()
        {
            string query = @"select EmployeeId,EmployeeName,Department,
                             convert(varchar(10),DateOfJoining,120) as DateOfJoining,PhotoFileName                            
                             from dbo.Employee";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommannd = new SqlCommand(query, myCon))
                {
                    myReader = myCommannd.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult(table);

        }
        [HttpPost]
        public JsonResult Post(Employee emp)
        {
            string query = @"insert into dbo.Employee
                            (EmployeeName,Department,DateOfJoining,PhotoFileName)
                            values (@EmployeeName,@Department,@DateOfJoining,@PhotoFileName)";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommannd = new SqlCommand(query, myCon))
                {
                    myCommannd.Parameters.AddWithValue("@EmployeeName", emp.EmployeeName);
                    myCommannd.Parameters.AddWithValue("@Department", emp.Department);
                    myCommannd.Parameters.AddWithValue("@DateOfJoining", emp.DateOfJoining);
                    myCommannd.Parameters.AddWithValue("@PhotoFileName", emp.PhotoFileName);
                    myReader = myCommannd.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult("Added Successfully");

        }
        [HttpPut]
        public JsonResult Put(Employee emp)
        {
            string query = @"Update dbo.Employee set
                             EmployeeName=@EmployeeName,
                             Department=@Department,
                             DateOfJoining=@DateOfJoining,
                             PhotoFileName=@PhotoFileName
                             where
                             EmployeeId=@EmployeeId";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommannd = new SqlCommand(query, myCon))
                {
                    myCommannd.Parameters.AddWithValue("@EmployeeId", emp.EmployeeId);
                    myCommannd.Parameters.AddWithValue("@EmployeeName", emp.EmployeeName);
                    myCommannd.Parameters.AddWithValue("@Department", emp.Department);
                    myCommannd.Parameters.AddWithValue("@DateOfJoining", emp.DateOfJoining);
                    myCommannd.Parameters.AddWithValue("@PhotoFileName", emp.PhotoFileName);
                    myReader = myCommannd.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult("Update Successfully");

        }
        [HttpDelete("{id}")]
        public JsonResult Delete(int id)
        {
            string query = @"delete from dbo.Employee where EmployeeId=@EmployeeId ";

            DataTable table = new DataTable();
            string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommannd = new SqlCommand(query, myCon))
                {
                    myCommannd.Parameters.AddWithValue("@EmployeeId", id);
                    myReader = myCommannd.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return new JsonResult("Delete Successfully");

        }
        [Route("SaveFile")]
        [HttpPost]
        public JsonResult SaveFile()
        {
            try
            {
                var httpRequest = Request.Form;
                var postedFile = httpRequest.Files[0];
                string filename = postedFile.FileName;
                var physicalPath = _env.ContentRootPath + "/Photos/" + filename;

                using (var stream = new FileStream(physicalPath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                return new JsonResult(filename);
            }
            catch (Exception)
            {

                return new JsonResult("anonymous.png");
            }
        }
    }
}
