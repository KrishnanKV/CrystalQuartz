﻿namespace CrystalQuartz.Samples.RemoteScheduler.Server
{
    using System;
    using System.Collections.Specialized;
    using CrystalQuarts.Samples.Common;
    using Quartz;
    using Quartz.Impl;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting scheduler...");

            var properties = new NameValueCollection();
            properties["quartz.scheduler.instanceName"] = "RemoteServerSchedulerClient";

            // set thread pool info
            properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
            properties["quartz.threadPool.threadCount"] = "5";
            properties["quartz.threadPool.threadPriority"] = "Normal";

            // set remoting expoter
            properties["quartz.scheduler.exporter.type"] = "Quartz.Simpl.RemotingSchedulerExporter, Quartz";
            properties["quartz.scheduler.exporter.port"] = "555";
            properties["quartz.scheduler.exporter.bindName"] = "QuartzScheduler";
            properties["quartz.scheduler.exporter.channelType"] = "tcp";

            var schedulerFactory = new StdSchedulerFactory(properties);
            var scheduler = schedulerFactory.GetScheduler();

            var map = new JobDataMap();
            map.Put("msg", "Some message!");

            var job = JobBuilder.Create<PrintMessageJob>()
                .WithIdentity("localJob", "default")
                .UsingJobData(map)
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("remotelyAddedTrigger", "default")
                .ForJob(job)
                .StartNow()
                .WithCronSchedule("/5 * * ? * *")
                .Build();

            // schedule the job
            scheduler.ScheduleJob(job, trigger);

            scheduler.Start();

            Console.WriteLine("Scheduler has been started.");
        }
    }
}
