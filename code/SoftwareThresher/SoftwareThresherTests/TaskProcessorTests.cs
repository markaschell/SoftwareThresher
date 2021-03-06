﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using SoftwareThresher;
using SoftwareThresher.Configurations;
using SoftwareThresher.Observations;
using SoftwareThresher.Reporting;
using SoftwareThresher.Settings.Search;
using SoftwareThresher.Tasks;

namespace SoftwareThresherTests {
   [TestClass]
   public class TaskProcessorTests {
      IConfigurationLoader configurationLoader;
      IConfiguration configuration;
      Task task;

      Report report;

      TaskProcessor taskProcessor;

      [TestInitialize]
      public void Setup() {
         configurationLoader = Substitute.For<IConfigurationLoader>();
         configuration = Substitute.For<IConfiguration>();
         task = Substitute.For<Task>();

         report = Substitute.For<Report>();

         taskProcessor = new TaskProcessor(configurationLoader, report);
      }

      static Observation ObservationStub => Substitute.For<Observation>((Search)null);

      [TestMethod]
      public void Run_MultipleTasks() {
         var task2 = Substitute.For<Task>();
         const string configurationnFilename = "config this";

         configurationLoader.Load(configurationnFilename).Returns(configuration);
         configuration.Tasks.Returns(new List<Task> { task, task2 });

         var passedObservation = ObservationStub;
         passedObservation.Failed.Returns(false);
         var failedObservation = ObservationStub;
         failedObservation.Failed.Returns(true);

         task.Execute(Arg.Is<List<Observation>>(l => l.Count == 0)).Returns(new List<Observation> { failedObservation, passedObservation });
         task2.Execute(Arg.Is<List<Observation>>(l => l.Count == 1)).Returns(new List<Observation>());

         taskProcessor.Run(configurationnFilename);

         task2.Received().Execute(Arg.Is<List<Observation>>(l => l.Count == 1 && l.First() == passedObservation));
      }

      [TestMethod]
      public void Run_WriteObservations() {
         const string configurationnFilename = "config this";

         configurationLoader.Load(Arg.Any<string>()).Returns(configuration);
         configuration.Tasks.Returns(new List<Task> { task });

         const string title = "This is it";
         task.ReportHeader.Returns(title);

         var passedObservation = ObservationStub;
         passedObservation.Failed.Returns(false);
         var failedObservation = ObservationStub;
         failedObservation.Failed.Returns(true);

         task.Execute(Arg.Any<List<Observation>>()).Returns(new List<Observation>());

         taskProcessor.Run(configurationnFilename);

         Received.InOrder(() => {
            report.Start(configurationnFilename);
            report.WriteObservations(title, 0, 0, Arg.Any<TimeSpan>(), Arg.Is<List<Observation>>(l => l.Count == 0));
            report.Complete();
         });
      }

      [TestMethod]
      public void Run_WriteFailedObservations() {
         configurationLoader.Load(Arg.Any<string>()).Returns(configuration);
         configuration.Tasks.Returns(new List<Task> { task });

         var passedObservation = ObservationStub;
         passedObservation.Failed.Returns(false);
         var failedObservation = ObservationStub;
         failedObservation.Failed.Returns(true);

         task.Execute(Arg.Any<List<Observation>>()).Returns(new List<Observation> { passedObservation, failedObservation, passedObservation });

         taskProcessor.Run("");

         report.Received().WriteObservations(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<TimeSpan>(), Arg.Is<List<Observation>>(l => l.Count == 1 && l.First() == failedObservation));
      }

      [TestMethod]
      public void Run_WriteDeltaObservations() {
         var task2 = Substitute.For<Task>();

         configurationLoader.Load(Arg.Any<string>()).Returns(configuration);
         configuration.Tasks.Returns(new List<Task> { task, task2 });

         var observation = ObservationStub;
         observation.Failed.Returns(false);
         var failedObservation = ObservationStub;
         failedObservation.Failed.Returns(true);

         task.Execute(Arg.Any<List<Observation>>()).Returns(new List<Observation> { observation, failedObservation });
         task2.Execute(Arg.Any<List<Observation>>()).Returns(new List<Observation> { observation, failedObservation, observation, observation });

         taskProcessor.Run("");

         Received.InOrder(() => {
            report.WriteObservations(Arg.Any<string>(), 1, Arg.Any<int>(), Arg.Any<TimeSpan>(), Arg.Any<List<Observation>>());
            report.WriteObservations(Arg.Any<string>(), 2, Arg.Any<int>(), Arg.Any<TimeSpan>(), Arg.Any<List<Observation>>());
         });
      }

      [TestMethod]
      public void Run_WritePassedObservations() {
         var task2 = Substitute.For<Task>();

         configurationLoader.Load(Arg.Any<string>()).Returns(configuration);
         configuration.Tasks.Returns(new List<Task> { task, task2 });

         var passedObservation = ObservationStub;
         passedObservation.Failed.Returns(false);
         var failedObservation = ObservationStub;
         failedObservation.Failed.Returns(true);

         task.Execute(Arg.Any<List<Observation>>()).Returns(new List<Observation> { passedObservation });
         task2.Execute(Arg.Any<List<Observation>>()).Returns(new List<Observation> { passedObservation, failedObservation, passedObservation });

         taskProcessor.Run("");

         Received.InOrder(() => {
            report.WriteObservations(Arg.Any<string>(), Arg.Any<int>(), 1, Arg.Any<TimeSpan>(), Arg.Any<List<Observation>>());
            report.WriteObservations(Arg.Any<string>(), Arg.Any<int>(), 2, Arg.Any<TimeSpan>(), Arg.Any<List<Observation>>());
         });
      }

      [TestMethod]
      public void Run_ErrorRunningTask_ReportIsFinialized() {
         configurationLoader.Load(Arg.Any<string>()).Returns(configuration);
         configuration.Tasks.Returns(new List<Task> { task });

         var exception = new Exception();
         task.Execute(Arg.Any<List<Observation>>()).Returns(x => { throw exception; });

         try {
            taskProcessor.Run("");

            Assert.Fail("Should have thrown an exception");
         }
         catch (Exception e) {
            report.Received().Complete();

            Assert.AreSame(exception, e);
         }
      }
   }
}
