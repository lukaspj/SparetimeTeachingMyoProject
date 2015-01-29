#r "MyoSharp.dll"
#load "ConsoleHelper.csx"

using MyoSharp.Communication;
using MyoSharp.Device;
using MyoSharp.Poses;
using MyoSharp.Exceptions;

// create a hub that will manage Myo devices for us
using (var channel = Channel.Create(
    ChannelDriver.Create(ChannelBridge.Create(),
    MyoErrorHandlerDriver.Create(MyoErrorHandlerBridge.Create()))))
using (var hub = Hub.Create(channel))
{
    // listen for when the Myo connects
    hub.MyoConnected += (sender, e) =>
    {
        Console.WriteLine("Myo {0} has connected!", e.Myo.Handle);
        e.Myo.Vibrate(VibrationType.Short);
        e.Myo.PoseChanged += Myo_PoseChanged;
        e.Myo.Locked += Myo_Locked;
        e.Myo.Unlocked += Myo_Unlocked;
    };

    // listen for when the Myo disconnects
    hub.MyoDisconnected += (sender, e) =>
    {
        Console.WriteLine("Oh no! It looks like {0} arm Myo has disconnected!", e.Myo.Arm);
        e.Myo.PoseChanged -= Myo_PoseChanged;
        e.Myo.Locked -= Myo_Locked;
        e.Myo.Unlocked -= Myo_Unlocked;
    };

    // start listening for Myo data
    channel.StartListening();

    // wait on user input
    ConsoleHelper.UserInputLoop(hub);
}

#region Event Handlers
private static void Myo_PoseChanged(object sender, PoseEventArgs e)
{
    Console.WriteLine("{0} arm Myo detected {1} pose!", e.Myo.Arm, e.Myo.Pose);
}

private static void Myo_Unlocked(object sender, MyoEventArgs e)
{
    Console.WriteLine("{0} arm Myo has unlocked!", e.Myo.Arm);
}

private static void Myo_Locked(object sender, MyoEventArgs e)
{
    Console.WriteLine("{0} arm Myo has locked!", e.Myo.Arm);
}
#endregion
