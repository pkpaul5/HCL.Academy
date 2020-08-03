using HCL.Academy.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Web.UI.DataVisualization.Charting;
using System.Net.Http;
using System.Threading.Tasks;
using HCLAcademy.Util;
using Microsoft.ApplicationInsights;
using System.Diagnostics;
namespace HCLAcademy.Controllers
{
    public class ChartsController : BaseController
    {
        /// <summary>
        /// Gets the heat map of the selected project and competency.
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="competency"></param>
        /// <returns></returns>
        public async Task<ActionResult> HeatMap(int projectId, string competency)
        {
            var imgStream = new MemoryStream();
            var heatMapChart = new Chart()
            {
                Width = 600,
                Height = 300
            };
            try
            {
                InitializeServiceClient();
                UserProjectRequest userProjectInfo = new UserProjectRequest();
                userProjectInfo.ProjectId = projectId;
                userProjectInfo.ClientInfo = req.ClientInfo;
                HttpResponseMessage trainingResponse = await client.PostAsJsonAsync("Project/GetResourceDetailsByProjectID", userProjectInfo);
                Resource prjRes = await trainingResponse.Content.ReadAsAsync<Resource>();

                List<string> xValues = new List<string>();
                List<double> yValues = new List<double>();
                List<double> yValues2 = new List<double>();

                for (int k = 0; k < prjRes.allResources.Count; k++)
                {
                    xValues.Add(prjRes.allResources[k].skill);
                    if (competency.ToUpper() == "BEGINNER" || competency.ToUpper() == "NOVICE")
                    {
                        yValues.Add(prjRes.allResources[k].expectedBeginnerCount);
                        yValues2.Add(prjRes.allResources[k].availableBeginnerCount);

                    }
                    else if (competency.ToUpper() == "ADVANCEDBEGINNER")
                    {
                        yValues.Add(prjRes.allResources[k].expectedAdvancedBeginnerCount);
                        yValues2.Add(prjRes.allResources[k].availableAdvancedBeginnerCount);
                    }
                    else if (competency.ToUpper() == "COMPETENT")
                    {
                        yValues.Add(prjRes.allResources[k].expectedCompetentCount);
                        yValues2.Add(prjRes.allResources[k].availableCompetentCount);
                    }
                    else if (competency.ToUpper() == "PROFICIENT")
                    {
                        yValues.Add(prjRes.allResources[k].expectedProficientCount);
                        yValues2.Add(prjRes.allResources[k].availableProficientCount);
                    }
                    else if (competency.ToUpper() == "EXPERT")
                    {
                        yValues.Add(prjRes.allResources[k].expectedExpertCount);
                        yValues2.Add(prjRes.allResources[k].availableExpertCount);
                    }
                }

                Series s1 = new Series();
                s1.Name = "Expected";
                s1.ChartType = SeriesChartType.Radar;
                s1.MarkerBorderColor = System.Drawing.Color.FromArgb(64, 64, 64);
                s1.MarkerSize = 9;
                s1.BorderColor = System.Drawing.Color.FromArgb(180, 26, 59, 105);
                s1.Color = System.Drawing.Color.FromArgb(220, 65, 140, 240);
                s1.ShadowOffset = 1;
                heatMapChart.Series.Add(s1);

                Series s2 = new Series();
                s2.Name = "Available";
                s2.ChartType = SeriesChartType.Radar;
                s2.MarkerBorderColor = System.Drawing.Color.FromArgb(64, 64, 64);
                s2.MarkerSize = 9;
                s2.BorderColor = System.Drawing.Color.FromArgb(180, 26, 59, 105);
                s2.Color = System.Drawing.Color.FromArgb(220, 252, 180, 65);
                s2.ShadowOffset = 1;
                heatMapChart.Series.Add(s2);

                heatMapChart.Series["Expected"].Points.DataBindXY(xValues, yValues);
                heatMapChart.Series["Available"].Points.DataBindXY(xValues, yValues2);
                heatMapChart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;

                Legend l = new Legend();
                l.IsTextAutoFit = false;
                l.Name = "Default";
                l.BackColor = System.Drawing.Color.Transparent;
                l.Font = new System.Drawing.Font("Trebuchet MS", 8, System.Drawing.FontStyle.Bold);
                l.Alignment = System.Drawing.StringAlignment.Far;
                l.Position.Y = 74;
                l.Position.Height = 14;
                l.Position.Width = 19;
                l.Position.X = 74;
                heatMapChart.Legends.Add(l);
                ChartArea c = new ChartArea();
                c.BorderColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
                c.BackSecondaryColor = System.Drawing.Color.White;
                c.BackColor = System.Drawing.Color.OldLace;
                c.ShadowColor = System.Drawing.Color.Transparent;
                c.Area3DStyle.Rotation = 10;
                c.Area3DStyle.Perspective = 10;
                c.Area3DStyle.Inclination = 15;
                c.Area3DStyle.IsRightAngleAxes = false;
                c.Area3DStyle.WallWidth = 0;
                c.Area3DStyle.IsClustered = false;
                c.Position.Y = 15;
                c.Position.Height = 78;
                c.Position.Width = 88;
                c.Position.X = 5;
                c.AxisY.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
                c.AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8, System.Drawing.FontStyle.Bold);
                c.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
                c.AxisX.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
                c.AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8, System.Drawing.FontStyle.Bold);
                c.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
                heatMapChart.ChartAreas.Add(c);

                // Save the chart to a MemoryStream                
                heatMapChart.SaveImage(imgStream, ChartImageFormat.Png);
                imgStream.Seek(0, SeekOrigin.Begin);
            }
            catch (Exception ex)
            {
                //     UserManager users = (UserManager)Session["CurrentUser"];
                //   LogHelper.AddLog("ChartsController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

            }
            // Return the contents of the Stream to the client
            return File(imgStream, "image/png");
        }
        /// <summary>
        /// Fetches the average of the selected project
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public async Task<ActionResult> AverageHeatMap(int projectId)
        {
            var heatMapChart = new Chart()
            {
                Width = 600,
                Height = 300
            };
            var imgStream = new MemoryStream();
            try
            {
                //IDAL dal = (new DALFactory()).GetInstance();
                //Resource prjRes = dal.GetResourceDetailsByProjectID(projectId);

                InitializeServiceClient();
                UserProjectRequest userProjectInfo = new UserProjectRequest();
                userProjectInfo.ProjectId = projectId;
                userProjectInfo.ClientInfo = req.ClientInfo;
                HttpResponseMessage trainingResponse = await client.PostAsJsonAsync("Project/GetResourceDetailsByProjectID", userProjectInfo);
                Resource prjRes = await trainingResponse.Content.ReadAsAsync<Resource>();

                List<string> xValues = new List<string>();
                List<double> yValues = new List<double>();
                List<double> yValues2 = new List<double>();

                for (int k = 0; k < prjRes.allResources.Count; k++)
                {
                    xValues.Add(prjRes.allResources[k].skill);

                    double expectedTotal = prjRes.allResources[k].expectedBeginnerCount + prjRes.allResources[k].expectedAdvancedBeginnerCount * 2 + prjRes.allResources[k].expectedCompetentCount * 3 + prjRes.allResources[k].expectedProficientCount * 4 + prjRes.allResources[k].expectedExpertCount * 5;
                    double expectedAverage = expectedTotal / (prjRes.allResources[k].expectedBeginnerCount + prjRes.allResources[k].expectedAdvancedBeginnerCount + prjRes.allResources[k].expectedCompetentCount + prjRes.allResources[k].expectedProficientCount + prjRes.allResources[k].expectedExpertCount);
                    yValues.Add(expectedAverage);

                    double availableTotal = prjRes.allResources[k].availableBeginnerCount + prjRes.allResources[k].availableAdvancedBeginnerCount * 2 + prjRes.allResources[k].availableCompetentCount * 3 + prjRes.allResources[k].availableProficientCount * 4 + prjRes.allResources[k].availableExpertCount * 5;
                    double availableAverage = availableTotal / (prjRes.allResources[k].availableBeginnerCount + prjRes.allResources[k].availableAdvancedBeginnerCount + prjRes.allResources[k].availableCompetentCount + prjRes.allResources[k].availableProficientCount + prjRes.allResources[k].availableExpertCount);

                    yValues2.Add(availableAverage);
                }

                Series s1 = new Series();
                s1.Name = "Expected";
                s1.ChartType = SeriesChartType.Radar;
                s1.MarkerBorderColor = System.Drawing.Color.FromArgb(64, 64, 64);
                s1.MarkerSize = 9;
                s1.BorderColor = System.Drawing.Color.FromArgb(180, 26, 59, 105);
                s1.Color = System.Drawing.Color.FromArgb(220, 65, 140, 240);
                s1.ShadowOffset = 1;
                heatMapChart.Series.Add(s1);

                Series s2 = new Series();
                s2.Name = "Available";
                s2.ChartType = SeriesChartType.Radar;
                s2.MarkerBorderColor = System.Drawing.Color.FromArgb(64, 64, 64);
                s2.MarkerSize = 9;
                s2.BorderColor = System.Drawing.Color.FromArgb(180, 26, 59, 105);
                s2.Color = System.Drawing.Color.FromArgb(220, 252, 180, 65);
                s2.ShadowOffset = 1;
                heatMapChart.Series.Add(s2);

                heatMapChart.Series["Expected"].Points.DataBindXY(xValues, yValues);
                heatMapChart.Series["Available"].Points.DataBindXY(xValues, yValues2);
                heatMapChart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;

                Legend l = new Legend();
                l.IsTextAutoFit = false;
                l.Name = "Default";
                l.BackColor = System.Drawing.Color.Transparent;
                l.Font = new System.Drawing.Font("Trebuchet MS", 8, System.Drawing.FontStyle.Bold);
                l.Alignment = System.Drawing.StringAlignment.Far;
                l.Position.Y = 74;
                l.Position.Height = 14;
                l.Position.Width = 19;
                l.Position.X = 74;
                heatMapChart.Legends.Add(l);
                ChartArea c = new ChartArea();
                c.BorderColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
                c.BackSecondaryColor = System.Drawing.Color.White;
                c.BackColor = System.Drawing.Color.OldLace;
                c.ShadowColor = System.Drawing.Color.Transparent;
                c.Area3DStyle.Rotation = 10;
                c.Area3DStyle.Perspective = 10;
                c.Area3DStyle.Inclination = 15;
                c.Area3DStyle.IsRightAngleAxes = false;
                c.Area3DStyle.WallWidth = 0;
                c.Area3DStyle.IsClustered = false;
                c.Position.Y = 15;
                c.Position.Height = 78;
                c.Position.Width = 88;
                c.Position.X = 5;
                c.AxisY.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
                c.AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8, System.Drawing.FontStyle.Bold);
                c.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
                c.AxisX.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
                c.AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8, System.Drawing.FontStyle.Bold);
                c.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
                heatMapChart.ChartAreas.Add(c);

                // Save the chart to a MemoryStream

                heatMapChart.SaveImage(imgStream, ChartImageFormat.Png);
                imgStream.Seek(0, SeekOrigin.Begin);
            }
            catch (Exception ex)
            {
                //UserManager users = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("ChartsController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);
            }
            // Return the contents of the Stream to the client
            return File(imgStream, "image/png");
        }


        public async Task<ActionResult> SkillMap(int competency,string projectId)
        {
            var imgStream = new MemoryStream();
            var skillMapChart = new Chart()
            {
                Width = 500,
                Height = 550
            };
            InitializeServiceClient();
            try
            {
                HttpResponseMessage skillResponse = await client.PostAsJsonAsync("Skill/GetAllSkillResourceCount?projectId=" + projectId, req);
                List<SkillCompetencyResource> result = await skillResponse.Content.ReadAsAsync<List<SkillCompetencyResource>>();
                List<string> xValues = new List<string>();
                List<double> yValues1 = new List<double>();
                List<double> yValues2 = new List<double>();
                List<double> yValues3 = new List<double>();
                List<double> yValues4 = new List<double>();
                List<double> yValues5 = new List<double>();

                for (int k = 0; k < result.Count; k++)
                {
                    xValues.Add(result[k].Skill);
                    yValues1.Add(result[k].NoviceCount);
                    yValues2.Add(result[k].AdvancedBeginnerCount);
                    yValues3.Add(result[k].CompetentCount);
                    yValues4.Add(result[k].ProficientCount);
                    yValues5.Add(result[k].ExpertCount);
                }

                Series s1 = new Series();
                s1.Name = "SkillMap";
                s1.ChartType = SeriesChartType.Radar;
                s1.MarkerBorderColor = System.Drawing.Color.FromArgb(64, 64, 64);
                s1.MarkerSize = 9;
                s1.BorderColor = System.Drawing.Color.FromArgb(180, 26, 59, 105);
                s1.Color = System.Drawing.Color.FromArgb(220, 65, 140, 240);
                s1.ShadowOffset = 1;
                skillMapChart.Series.Add(s1);

                //Legend l = new Legend();
                //l.IsTextAutoFit = false;
                //l.Name = "Default";
                //l.BackColor = System.Drawing.Color.Transparent;
                //l.Font = new System.Drawing.Font("Trebuchet MS", 7, System.Drawing.FontStyle.Bold);
                //l.Alignment = System.Drawing.StringAlignment.Far;
                //l.Position.Y = 74;
                //l.Position.Height = 14;
                //l.Position.Width = 19;
                //l.Position.X = 74;

                switch (competency)
                {
                    case 1:
                        //Series s1 = new Series();
                        //s1.Name = "Novice";
                        //s1.ChartType = SeriesChartType.Radar;
                        //s1.MarkerBorderColor = System.Drawing.Color.FromArgb(64, 64, 64);
                        //s1.MarkerSize = 9;
                        //s1.BorderColor = System.Drawing.Color.FromArgb(220, 255, 255, 204);                        
                        //s1.Color = System.Drawing.Color.FromArgb(220, 255, 255, 204);
                        //s1.ShadowOffset = 1;
                        //skillMapChart.Series.Add(s1);
                        skillMapChart.Series["SkillMap"].Points.DataBindXY(xValues, yValues1);
                        //l.Title = "Novice";
                        break;

                    case 2:
                        //Series s2 = new Series();
                        //s2.Name = "AdvancedBeginner";
                        //s2.ChartType = SeriesChartType.Radar;
                        //s2.MarkerBorderColor = System.Drawing.Color.FromArgb(64, 64, 64);
                        //s2.MarkerSize = 9;
                        //s2.BorderColor = System.Drawing.Color.FromArgb(220, 255, 255, 204);
                        ////s2.BorderColor = System.Drawing.Color.FromArgb(220, 252, 180, 65);
                        //s2.Color = System.Drawing.Color.FromArgb(220, 255, 255, 204);
                        //s2.ShadowOffset = 1;
                        //skillMapChart.Series.Add(s2);
                        skillMapChart.Series[0].Points.DataBindXY(xValues, yValues2);
                       // l.Title = "Advanced Beginner";
                        break;

                    case 3:
                        //Series s3 = new Series();
                        //s3.Name = "Competent";
                        //s3.ChartType = SeriesChartType.Radar;
                        //s3.MarkerBorderColor = System.Drawing.Color.FromArgb(64, 64, 64);
                        //s3.MarkerSize = 9;
                        //s3.BorderColor = System.Drawing.Color.FromArgb(220, 255, 204, 153);
                        ////s3.BorderColor = System.Drawing.Color.FromArgb(220, 63, 191, 127);
                        //s3.Color = System.Drawing.Color.FromArgb(220, 255, 204, 153);
                        //s3.ShadowOffset = 1;
                        //skillMapChart.Series.Add(s3);
                        skillMapChart.Series[0].Points.DataBindXY(xValues, yValues3);
                     //   l.Title = "Competent";
                        break;

                    case 4:
                        //Series s4 = new Series();
                        //s4.Name = "Proficient";
                        //s4.ChartType = SeriesChartType.Radar;
                        //s4.MarkerBorderColor = System.Drawing.Color.FromArgb(64, 64, 64);
                        //s4.MarkerSize = 9;
                        //s4.BorderColor = System.Drawing.Color.FromArgb(220, 255, 204, 229);
                        ////s4.BorderColor = System.Drawing.Color.FromArgb(220, 63, 127, 191);
                        //s4.Color = System.Drawing.Color.FromArgb(220, 255, 204, 229);
                        //s4.ShadowOffset = 1;
                        //skillMapChart.Series.Add(s4);
                        skillMapChart.Series[0].Points.DataBindXY(xValues, yValues4);
                   //     l.Title = "Proficient";
                        break;

                    case 5:
                        //Series s5 = new Series();
                        //s5.Name = "Expert";
                        //s5.ChartType = SeriesChartType.Radar;
                        //s5.MarkerBorderColor = System.Drawing.Color.FromArgb(64, 64, 64);
                        //s5.MarkerSize = 9;
                        //s5.BorderColor = System.Drawing.Color.FromArgb(220, 153, 255, 153);
                        ////s5.BorderColor = System.Drawing.Color.FromArgb(220, 191, 63, 63);
                        //s5.Color = System.Drawing.Color.FromArgb(220, 153, 255, 153);
                        //s5.ShadowOffset = 1;
                        //skillMapChart.Series.Add(s5);
                        skillMapChart.Series[0].Points.DataBindXY(xValues, yValues5);
                 //       l.Title = "Expert";
                        break;

                }
                skillMapChart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;

                
                
               // skillMapChart.Legends.Add(l);
                ChartArea c = new ChartArea();
                c.BorderColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
                c.BackSecondaryColor = System.Drawing.Color.White;
                c.BackColor = System.Drawing.Color.OldLace;
                c.ShadowColor = System.Drawing.Color.Transparent;
                c.Area3DStyle.Rotation = 10;
                c.Area3DStyle.Perspective = 10;
                c.Area3DStyle.Inclination = 15;
                c.Area3DStyle.IsRightAngleAxes = false;
                c.Area3DStyle.WallWidth = 0;
                c.Area3DStyle.IsClustered = false;
                c.Position.Y = 15;
                c.Position.Height = 78;
                c.Position.Width = 88;
                c.Position.X = 5;
                c.AxisY.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
                c.AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 7, System.Drawing.FontStyle.Bold);
                c.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
                c.AxisX.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
                c.AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 7, System.Drawing.FontStyle.Bold);
                c.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(64, 64, 64, 64);
                skillMapChart.ChartAreas.Add(c);

                // Save the chart to a MemoryStream                
                
                skillMapChart.SaveImage(imgStream, ChartImageFormat.Png);
                imgStream.Seek(0, SeekOrigin.Begin);
            }
            catch (Exception ex)
            {
                //UserManager users = (UserManager)Session["CurrentUser"];
                //LogHelper.AddLog("ChartsController", ex.Message, ex.StackTrace, "HCL.Academy.Web", user.EmailID);
                TelemetryClient telemetry = new TelemetryClient();
                telemetry.TrackException(ex);

            }
            // Return the contents of the Stream to the client
            return File(imgStream, "image/png");
        }
    }
}
