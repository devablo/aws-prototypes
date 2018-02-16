using System;

namespace Devablo.Prototypes.Amazon.SQS.Core.Domain
{
    public class EditStationCommand : ICommand
    {
        public Guid Uid { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
    }
}