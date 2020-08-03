using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using HCL.Academy.DAL;
using HCL.Academy.Model;

namespace HCLAcademyEmailJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see https://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            var config = new JobHostConfiguration();

            if (config.IsDevelopment)
            {
                config.UseDevelopmentSettings();
            }

            var host = new JobHost(config);
            ServiceConsumerInfo consumer = new ServiceConsumerInfo();
            consumer.emailId = "";

            SqlSvrDAL dal = new SqlSvrDAL(consumer);
            bool result = false;
            bool reminder = false;
            result = dal.ProcessRemiderEmail();
            reminder = dal.ProcessEscalationEmail();
            // The following code ensures that the WebJob will be running continuously
            host.RunAndBlock();
        }
    }
}
