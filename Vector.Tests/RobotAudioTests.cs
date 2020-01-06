using Microsoft.VisualStudio.TestTools.UnitTesting;
using Vector;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Vector.Tests
{
    [TestClass()]
    public class RobotAudioTests
    {
        [TestMethod()]
        public async Task SayTextAsyncTest()
        {
            var robot = await RobotTests.GetRobot();
            await robot.Audio.SayTextAsync("good morning world!  It's a beautiful day");
        }
    }
}