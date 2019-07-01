namespace Willow.Application.Event
{
    using System;

    public interface IHoldEventState
    {
        Guid Id { get; set; }
        string Title { get; set; }
        string Description { get; set; }
    }
}