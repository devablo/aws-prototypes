using System;

namespace Devablo.Prototypes.Amazon.SQS.Core.Domain
{
    public class CreateStationCommand : ICommand
    {
        public string Name { get; set; }
        public string Code { get; set; }
    }
}