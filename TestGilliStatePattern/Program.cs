using System;

namespace TestGilliStatePattern
{
    class Program
    {
        static void Main(string[] args)
        {

            // TODO: implement tasks in order to wait a given state of the sub context before to change to the next
            // state
            var contextNesting = new CameraNesting.NestingContext(new CameraNesting.IdleState());
            var robot = new Robot.RobotContext(new Robot.IdleState(), contextNesting);
            var pallet = new CameraPallet.PalletContext(new CameraPallet.IdleState(), robot);
            pallet.StartCycle();


        }
    }
}
