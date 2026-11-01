using FluentValidation;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarTabDevices;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabDevices.Commands.CreateGuitarTabDevice;

internal sealed class CreateGuitarTabDeviceCommandValidator
    : AbstractValidator<CreateGuitarTabDeviceCommand>
{
    public CreateGuitarTabDeviceCommandValidator()
    {
        RuleFor(command => command.DeviceId).NotEmpty().WithMessage("Device id is required.");

        RuleFor(command => command.Name)
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(GuitarTabDevice.NameMaxLength)
            .WithMessage($"Name must be at most {GuitarTabDevice.NameMaxLength} characters.");

        RuleFor(command => command.Speed)
            .InclusiveBetween(GuitarTabDevice.MinSpeed, GuitarTabDevice.MaxSpeed)
            .WithMessage(
                $"Speed must be between {GuitarTabDevice.MinSpeed} and {GuitarTabDevice.MaxSpeed}."
            );

        RuleFor(command => command.Semitones)
            .InclusiveBetween(GuitarTabDevice.MinSemitones, GuitarTabDevice.MaxSemitones)
            .WithMessage(
                $"Transpose must be between {GuitarTabDevice.MinSemitones} and {GuitarTabDevice.MaxSemitones}."
            );
    }
}
