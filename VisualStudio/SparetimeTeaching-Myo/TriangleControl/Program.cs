using System;
using MyoSharp.Communication;
using MyoSharp.Device;
using MyoSharp.Exceptions;
using SharpDX.Windows;
using SparetimeTeachingLibrary;

namespace TriangleControl
{
   class Program
   {
      private static TriangleRenderData.PS_CONSTANT_BUF_DATA _psConstData;

      static void Main(string[] args)
      {
         DXSystem dxSystem = new DXSystem();
         dxSystem.InitializeDXSystem();

         TriangleRenderData triangleRenderData = new TriangleRenderData(dxSystem);
         triangleRenderData.Initialize();

         _psConstData = new TriangleRenderData.PS_CONSTANT_BUF_DATA
         {
            roll = 0f,
            pitch = 0,
            yaw = 0f,
            filler = 0f
         };

         InitializeMyo();

         // Main loop
         RenderLoop.Run(dxSystem.form, () =>
         {
            triangleRenderData.UpdateConsts(ref _psConstData);

            triangleRenderData.Render();
         });

         // Release all resources
         dxSystem.DisposeDXSystem();
      }

      private static void InitializeMyo()
      {
         // create a hub that will manage Myo devices for us
         IChannel channel = Channel.Create(
            ChannelDriver.Create(ChannelBridge.Create(),
               MyoErrorHandlerDriver.Create(MyoErrorHandlerBridge.Create())));
         IHub hub = Hub.Create(channel);
         // listen for when the Myo connects
         hub.MyoConnected += (sender, e) =>
         {
            Console.WriteLine("Myo {0} has connected!", e.Myo.Handle);
            e.Myo.Vibrate(VibrationType.Short);
            e.Myo.PoseChanged += Myo_PoseChanged;
            e.Myo.Locked += Myo_Locked;
            e.Myo.Unlocked += Myo_Unlocked;
            e.Myo.OrientationDataAcquired += Myo_OrientationDataAcquired;
         };

         // listen for when the Myo disconnects
         hub.MyoDisconnected += (sender, e) =>
         {
            Console.WriteLine("Oh no! It looks like {0} arm Myo has disconnected!", e.Myo.Arm);
            e.Myo.PoseChanged -= Myo_PoseChanged;
            e.Myo.Locked -= Myo_Locked;
            e.Myo.Unlocked -= Myo_Unlocked;
            e.Myo.OrientationDataAcquired -= Myo_OrientationDataAcquired;
         };

         // start listening for Myo data
         channel.StartListening();
      }

      #region Event Handlers
      private static void Myo_PoseChanged(object pSender, PoseEventArgs pEvent)
      {
         Console.WriteLine("{0} arm Myo detected {1} pose!", pEvent.Myo.Arm, pEvent.Myo.Pose);
      }

      private static void Myo_Unlocked(object pSender, MyoEventArgs pEvent)
      {
         Console.WriteLine("{0} arm Myo has unlocked!", pEvent.Myo.Arm);
      }

      private static void Myo_Locked(object pSender, MyoEventArgs pEvent)
      {
         Console.WriteLine("{0} arm Myo has locked!", pEvent.Myo.Arm);
      }

      private static void Myo_OrientationDataAcquired(object pSender, OrientationDataEventArgs pEvent)
      {
         const float PI = (float)System.Math.PI;

         // convert the values to a 0-9 scale (for easier digestion/understanding)
         var roll = (int)((pEvent.Roll + PI) / (PI * 2.0f) * 10);
         var pitch = (int)((pEvent.Pitch + PI) / (PI * 2.0f) * 10);
         var yaw = (int)((pEvent.Yaw + PI) / (PI * 2.0f) * 10);

         Console.Clear();
         Console.WriteLine(@"Roll: {0}", pEvent.Roll);
         Console.WriteLine(@"Pitch: {0}", MathHelper.Clamp(Math.Abs(-pEvent.Pitch / PI*2.0f), 0, 1));
         Console.WriteLine(@"Yaw: {0}", pEvent.Yaw);
         _psConstData.roll = (float)MathHelper.Clamp(Math.Abs(-pEvent.Roll / PI * 2.0f), 0f, 1f);
         _psConstData.pitch = (float)MathHelper.Clamp(Math.Abs(-pEvent.Pitch / PI * 2.0f), 0f, 1f);
         _psConstData.yaw = (float)MathHelper.Clamp(Math.Abs(-pEvent.Yaw / PI * 2.0f), 0f, 1f);
      }
      #endregion
   }
}
