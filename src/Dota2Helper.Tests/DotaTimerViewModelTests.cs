using Dota2Helper.Features.Audio;
using Dota2Helper.Features.Settings;
using Dota2Helper.Features.Timers;
using Dota2Helper.ViewModels;

namespace Dota2Helper.Tests;

[TestClass]
public class DotaTimerViewModelTests
{
    [TestMethod]
    public void WhenTimer_IsManualReset_And_ReachesEnd_Then_RequiresManualReset()
    {
        // Arrange
        var timer = new DotaTimerViewModel(new DotaTimer
        {
            IsManualReset = true,
            Time = TimeSpan.FromSeconds(10),
            IsEnabled = true,
            IsInterval = true,
            Name = "Test Timer Manual Reset",
            IsMuted = false,
            AudioFile = null
        }, new FakeTimerAudioService());

        // Act
        TimeSpan time = TimeSpan.FromSeconds(0);
        TimeSpan maxTime = timer.Time;

        Assert.IsTrue(timer.IsFirstManualTimer);

        while (time < maxTime)
        {
            timer.Update(time);
            time = time.Add(TimeSpan.FromSeconds(1));
        }

        // Assert first manual reset is not required
        Assert.IsFalse(timer.IsResetRequired);
        Assert.IsFalse(timer.IsFirstManualTimer);

        time = TimeSpan.FromSeconds(10);
        maxTime = maxTime.Add(timer.Time);

        while (time < maxTime)
        {
            timer.Update(time);
            time = time.Add(TimeSpan.FromSeconds(1));
        }

        // Assert second manual reset is required
        Assert.IsFalse(timer.IsFirstManualTimer);
        Assert.IsTrue(timer.IsResetRequired);
    }
}