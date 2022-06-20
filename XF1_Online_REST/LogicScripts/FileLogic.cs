using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;

namespace XF1_Online_REST.LogicScripts
{
    public struct Result
    {
        public string xfiaCode;
        public string raceTeamName;
        public string pilotName;
        public string type;
        public string price;
        public string qualificationPosition;
        public string q1;
        public string q2;
        public string q3;
        public string qualifiedQualification;
        public string disqualifiedQualification;
        public string racePosition;
        public string fastestLap;
        public string teammate;
        public string qualifiedRace;
        public string disqualifiedRace;
    }

    sealed class ResultMap : ClassMap<Result>
    {
        public ResultMap()
        {
            Map(m => m.xfiaCode).Name("Codigo XFIA");
            Map(m => m.raceTeamName).Name("Constructor");
            Map(m => m.pilotName).Name("Nombre");
            Map(m => m.type).Name("Tipo");
            Map(m => m.price).Name("Precio");
            Map(m => m.qualificationPosition).Name("Posicion Calificacion");
            Map(m => m.q1).Name("Q1");
            Map(m => m.q2).Name("Q2");
            Map(m => m.q3).Name("Q3");
            Map(m => m.qualifiedQualification).Name("Sin Calificar Calificacion");
            Map(m => m.disqualifiedQualification).Name("Descalificado Calificacion");
            Map(m => m.racePosition).Name("Posicion Carrera");
            Map(m => m.fastestLap).Name("Vuelta mas rapida");
            Map(m => m.teammate).Name("Gano a companero de equipo");
            Map(m => m.qualifiedRace).Name("Sin Calificar Carrera");
            Map(m => m.disqualifiedRace).Name("Descalificado de Carrera");
        }
    }

    public class FileLogic
    {
        private XF1OnlineEntities dbContext;
        private Tools tools;
        List<Result> results;
        string filePath;

        public FileLogic()
        {
            this.dbContext = new XF1OnlineEntities();
            this.tools = new Tools();
            this.results= results = new List<Result>();
            filePath = Path.Combine(Path.Combine(System.AppContext.BaseDirectory, "uploadedFiles"), "raceResults.csv");
        }

        public void saveFile(HttpPostedFile file)
        {
            file.SaveAs(filePath);
        }

        public Boolean parseResults()
        {
            StreamReader streamReader = new StreamReader(this.filePath);
            CsvReader csvReader = new CsvReader(streamReader, CultureInfo.CurrentCulture);
            try
            {
                using (CsvReader reader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                {
                    reader.Context.RegisterClassMap<ResultMap>();
                    this.results = reader.GetRecords<Result>().ToList();
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public HttpResponseMessage uploadAndParseFile(HttpFileCollection files,bool updatePoints,bool updatePrice,string token,string salt)
        {
            Error_List errors = new Error_List();
            errors.addError("Invalid token", tools.verifyToken(token, salt, "Administrator"));
            if (!errors.hasErrors())
            {
                errors.addError("No files where uploaded", files.Count == 1);
                if(!errors.hasErrors())
                {
                    HttpPostedFile file = files[0];
                    this.saveFile(file);
                    bool correctFormatCond=this.parseResults();
                    errors.addError("The given file does not have a correct CSV format inside", correctFormatCond);
                    if (correctFormatCond)
                    {
                        bool correctDataCond = !updateValues(updatePrice, updatePoints);
                        errors.addError("Issues where found on the file, please check it and try uploading again", correctDataCond);
                        
                        if(correctDataCond)
                        {
                            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("Data updated succesfully") };
                        }
                        
                    }

                }
            }
            errors.purgeErrorsList();
            return new HttpResponseMessage(HttpStatusCode.Conflict) { Content = new StringContent(JsonConvert.SerializeObject(errors)) };
        }
        public bool updateValues(bool updatePrice,bool updatePoints)
        {
            bool issuesFound = false;
            foreach(Result result in this.results)
            {
                if(result.type=="Piloto")
                {
                    Pilot pilot = dbContext.Pilots.Find(result.pilotName);
                    if (updatePrice)
                    {
                        pilot.Price = decimal.Parse(result.price);
                    }
                    if(updatePoints)
                    {
                        issuesFound=assignPointsPilot(pilot, result);
                    }
                }
                else if(result.type=="Constructor")
                {
                    Racing_Team team = dbContext.Racing_Team.Find(result.raceTeamName);
                    if(updatePrice)
                    {
                        team.Price = decimal.Parse(result.price);
                    }
                }
                else
                {
                    issuesFound = true;
                }
            }
            if(issuesFound)
            {
                rollBack(this.dbContext);
            }
            else
            {
                dbContext.SaveChanges();
            }
            return issuesFound;
        }
        public Boolean assignPointsPilot(Pilot pilot, Result result)
        {
            Boolean issuesFound = false;

            int q1 = Convert.ToInt32(result.q1 == "Y");
            int q2 = Convert.ToInt32(result.q2 == "Y");
            int q3 = Convert.ToInt32(result.q3 == "Y");

            int nQQ= Convert.ToInt32(result.qualifiedQualification == "Y");
            int dQQ= Convert.ToInt32(result.disqualifiedQualification == "Y");

            int fastestLap = Convert.ToInt32(result.fastestLap=="Y");
            int qR= Convert.ToInt32(result.qualifiedRace == "Y");
            int nQR = Convert.ToInt32(result.qualifiedRace == "Y");
            int dQR = Convert.ToInt32(result.disqualifiedRace == "Y");

            int positionQ = Convert.ToInt32(result.qualificationPosition);
            int positionR = Convert.ToInt32(result.racePosition);

            issuesFound = positionQ < 1 && positionR < 1; 
            if (!issuesFound)
            {
                if (positionQ<=10)
                {
                    positionQ = 11 - positionQ;
                }
                if (positionR <= 10)
                {
                    int[] points = new int[] { 1, 2, 4, 6, 8, 10, 12, 15, 18, 25 };
                    positionR = points[10 - positionR];
                }
                else
                {
                    positionR = 0;
                }
                int tempPoints = (q1 * 1 +
                                        q2 * 2 +
                                        q3 * 3 +
                                      -5 * nQQ +
                                     -10 * dQQ +
                                      positionQ +
                                             qR +
                                   fastestLap * 5 +
                                        -10 * nQR +
                                        -20 * dQR +
                                     positionR);


                List<Team> teams = dbContext.Teams.Where(o => o.Pilots.FirstOrDefault(j=>j.Name==pilot.Name)!=null).ToList();
                foreach (Team team in teams)
                {
                    List<Score> scores = dbContext.Scores.Where(o => o.Username == team.Username).ToList();
                    foreach (Score score in scores)
                    {
                        
                        
                        int multiplier = 1;
                        if(team.Racing_Team_Name==pilot.Racing_Team)
                        {
                            multiplier = 2;
                        }
                        score.Points += multiplier*tempPoints;
                    }
                }
            }
            return issuesFound;

        }
        public static void rollBack(XF1OnlineEntities dbContext)
        {
            foreach (DbEntityEntry entry in dbContext.ChangeTracker.Entries())
            {
                if(entry.State==EntityState.Modified)
                {
                    entry.State = EntityState.Unchanged;
                }
            }
        }
    }
    
}