﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iflee.Models;
using Iflee.Psfs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Iflee.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AircraftsController : ControllerBase
    {
        private readonly IfleeContext ifleeContext;
        public AircraftsController(IfleeContext ifleeContext)
        {
            this.ifleeContext = ifleeContext;
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            string sqlQuery = @"DELETE FROM aircrafts WHERE 
                aircrafts.id = @aircraftId;";
            using (var dbTransaction
                = ifleeContext.Database.BeginTransaction())
            {
                try
                {
                    ifleeContext.Database.ExecuteSqlCommand(sqlQuery,
                        new NpgsqlParameter("@aircraftId", id));
                    ifleeContext.SaveChanges();
                    dbTransaction.Commit();
                    return Ok();
                }
                catch (Exception)
                {
                    dbTransaction.Rollback();
                    return StatusCode(500);
                }
            }
        }

        [HttpGet]
        public IActionResult Get([FromQuery] PsfReadAircrafts
            psfReadAircrafts)
        {
            StringBuilder sqlQueryBuilder = new StringBuilder(
                "SELECT * FROM aircrafts");
            List<NpgsqlParameter> sqlQueryParameters
                = new List<NpgsqlParameter>();
            if (psfReadAircrafts.LimitOffset > 0)
            {
                sqlQueryBuilder.Append(" OFFSET @limitOffset");
                sqlQueryParameters.Add(new NpgsqlParameter("@limitOffset",
                    psfReadAircrafts.LimitOffset));
            }
            if (psfReadAircrafts.LimitRowCount >= 0)
            {
                sqlQueryBuilder.Append(" LIMIT @limitRowCount");
                sqlQueryParameters.Add(new NpgsqlParameter("@limitRowCount",
                    psfReadAircrafts.LimitRowCount));
            }
            sqlQueryBuilder.Append(";");
            string sqlQuery = sqlQueryBuilder.ToString();
            try
            {
                var aircrafts = ifleeContext.Aircrafts.FromSql(sqlQuery,
                sqlQueryParameters.ToArray()).ToList();
                return Ok(aircrafts);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            string sqlQuery = @"SELECT * FROM aircrafts
                WHERE aircrafts.id = @aircraftId";
            try
            {
                var aircrafts = ifleeContext.Aircrafts.FromSql(sqlQuery,
                    new NpgsqlParameter("@aircraftId", id)).ToList();
                return Ok(aircrafts.FirstOrDefault());
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
            
        }

        [HttpGet("is-exists")]
        public IActionResult IsExists( [FromQuery] PsfReadAircraftsIsExists
            psfReadAircraftsIsExists)
        {
            if (!psfReadAircraftsIsExists.IsValid())
            {
                return BadRequest();
            }
            StringBuilder sqlQueryBuilder = new StringBuilder(
                @"SELECT 1 AS id, CASE WHEN COUNT(*) > 0 THEN TRUE ELSE FALSE 
                END AS value FROM aircrafts WHERE");
            List<NpgsqlParameter> sqlQueryParameters
                = new List<NpgsqlParameter>();
            if (psfReadAircraftsIsExists.IsTypeValid())
            {
                sqlQueryBuilder.Append(" type = @aircraftType");
                sqlQueryParameters.Add(new NpgsqlParameter("@aircraftType",
                    psfReadAircraftsIsExists.Type));
            }
            if (psfReadAircraftsIsExists.IsBoardNumberValid())
            {
                sqlQueryBuilder.Append(" board_number = @aircraftBoardNumber");
                sqlQueryParameters.Add(new NpgsqlParameter(
                    "@aircraftBoardNumber",
                    psfReadAircraftsIsExists.BoardNumber));
            }
            if (psfReadAircraftsIsExists.IsMarkValid())
            {
                sqlQueryBuilder.Append(" mark = @aircraftMark");
                sqlQueryParameters.Add(new NpgsqlParameter("@aircraftMark",
                    psfReadAircraftsIsExists.Mark));
            }
            if (psfReadAircraftsIsExists.IsModelValid())
            {
                sqlQueryBuilder.Append(" model = @aircraftModel");
                sqlQueryParameters.Add(new NpgsqlParameter("@aircraftModel",
                    psfReadAircraftsIsExists.Model));
            }
            sqlQueryBuilder.Append(";");
            string sqlQuery = sqlQueryBuilder.ToString();
            try
            {
                var aircraftsIsExists = ifleeContext.aircraftsIsExists.FromSql(
                    sqlQuery, sqlQueryParameters.ToArray()).ToList();
                return Ok(aircraftsIsExists.FirstOrDefault());
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] PsfCreateAircraft
            psfCreateAircraft)
        {
            if (!psfCreateAircraft.IsValid())
            {
                return BadRequest();
            }
            string sqlQuery = @"INSERT INTO aircrafts ( type, board_number,
                mark, model ) VALUES( @aircraftType, @aircraftBoardNumber,
                @aircraftMark, @aircraftModel );";
            using (var dbTransaction
                = ifleeContext.Database.BeginTransaction())
            {
                try
                {
                    ifleeContext.Database.ExecuteSqlCommand(sqlQuery,
                        new NpgsqlParameter("@aircraftType",
                            psfCreateAircraft.Type),
                        new NpgsqlParameter("@aircraftBoardNumber",
                            psfCreateAircraft.BoardNumber),
                        new NpgsqlParameter("@aircraftMark",
                            psfCreateAircraft.Mark),
                        new NpgsqlParameter("@aircraftModel",
                            psfCreateAircraft.Model));
                    ifleeContext.SaveChanges();
                    dbTransaction.Commit();
                    return Ok();
                }
                catch (Exception)
                {
                    dbTransaction.Rollback();
                    return BadRequest();
                }
            }
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromQuery] PsfUpdateAircraft
            psfUpdateAircraft)
        {
            if (!psfUpdateAircraft.IsValid()) { 
                return BadRequest();
            }
            StringBuilder sqlQueryBuilder = new StringBuilder(
                "UPDATE aircrafts SET");
            List<NpgsqlParameter> sqlQueryParameters
                = new List<NpgsqlParameter>() {
                    new NpgsqlParameter("@aircraftId", id)
                };
            if (psfUpdateAircraft.IsTypeValid())
            {
                sqlQueryBuilder.Append(" type = @aircraftType");
                sqlQueryParameters.Add(new NpgsqlParameter("@aircraftType",
                    psfUpdateAircraft.Type));
            }
            if (psfUpdateAircraft.IsBoardNumberValid())
            {
                sqlQueryBuilder.Append(" board_number = @aircraftBoardNumber");
                sqlQueryParameters.Add(new NpgsqlParameter(
                    "@aircraftBoardNumber", psfUpdateAircraft.BoardNumber));
            }
            if (psfUpdateAircraft.IsMarkValid())
            {
                sqlQueryBuilder.Append(" mark = @aircraftMark");
                sqlQueryParameters.Add(new NpgsqlParameter("@aircraftMark",
                    psfUpdateAircraft.Mark));
            }
            if (psfUpdateAircraft.IsModelValid())
            {
                sqlQueryBuilder.Append(" model = @aircraftModel");
                sqlQueryParameters.Add(new NpgsqlParameter("@aircraftModel",
                    psfUpdateAircraft.Model));
            }
            sqlQueryBuilder.Append(" WHERE aircrafts.id = @aircraftId;");
            string sqlQuery = sqlQueryBuilder.ToString();
            using (var dbTransaction
                = ifleeContext.Database.BeginTransaction())
            {
                try
                {
                    ifleeContext.Database.ExecuteSqlCommand(sqlQuery,
                        sqlQueryParameters.ToArray());
                    ifleeContext.SaveChanges();
                    dbTransaction.Commit();
                    return Ok();
                }
                catch (Exception)
                {
                    dbTransaction.Rollback();
                    return StatusCode(500);
                }
            }
        }

        [HttpGet("total")]
        public IActionResult Total()
        {
            string sqlQuery = @"SELECT 1 AS id, COUNT(*) AS value FROM 
                aircrafts;";
            try
            {
                var aircraftsTotals = ifleeContext.aircraftsTotals.FromSql(
                    sqlQuery).ToList();
                return Ok(aircraftsTotals.FirstOrDefault());
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}