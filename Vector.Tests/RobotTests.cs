using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vector;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using AutoMapper;

namespace Vector.Tests
{
    [TestClass()]
    public class RobotTests
    {
        const string RobotName = "A5A7";
        static ConcurrentDictionary<string, Task<Robot>> _robots = new ConcurrentDictionary<string, Task<Robot>>();

        public static async Task<Robot> GetRobot(string robotName = RobotName)
        {
            return await _robots.GetOrAdd(robotName, async i =>
            {
                var robot = new Robot();
                await robot.ConnectAsync("A5A7");
                robot.SuppressPersonalityAsync().ThrowFeedException();
                await robot.WaitTillPersonalitySuppressedAsync();
                return robot;
            });
        }

        public static async Task DisconnectRobot(string robotName = RobotName)
        {
            if (_robots.TryRemove(robotName, out var robotTask))
            {
                var robot = await robotTask;
                await robot.DisconnectAsync();
            }
        }

        [TestMethod()]
        public async Task ConnectAsyncTest()
        {
            var robot = await GetRobot();
            Assert.IsTrue(robot.IsConnected);
        }

        [TestMethod()]
        public async Task DisconnectAsyncTest()
        {
            await DisconnectRobot();
        }

        [TestMethod()]
        public async Task GetBatteryStateAsyncTest()
        {
            var robot = await GetRobot();
            var result = await robot.GetBatteryStateAsync();
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task GetVersionStateAsyncTest()
        {
            var robot = await GetRobot();
            var result = await robot.GetVersionStateAsync();
            Assert.IsNotNull(result);
        }

        [TestMethod()]
        public async Task EventListeningAsyncTest()
        {
            var robot = await GetRobot();
            var resultSource = new TaskCompletionSource<RobotState>();
            var cancel = new CancellationTokenSource();
            robot.OnStateChanged += (sender, e) => { resultSource.TrySetResult(e.Data); cancel.Cancel(); };
            await robot.EventListeningAsync(cancel.Token).ThrowFeedExceptionTask();
            Assert.IsTrue(resultSource.Task.IsCompleted);
            var result = resultSource.Task.Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task ValidateMapping()
        {
            var robot = await GetRobot();
            Mapper.Configuration.AssertConfigurationIsValid();
        }
    }
}