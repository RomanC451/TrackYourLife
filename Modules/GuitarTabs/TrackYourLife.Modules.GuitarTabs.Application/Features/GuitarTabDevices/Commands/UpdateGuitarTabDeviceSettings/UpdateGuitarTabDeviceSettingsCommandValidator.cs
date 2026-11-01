using FluentValidation;
using TrackYourLife.Modules.GuitarTabs.Domain.Features.GuitarTabDevices;

namespace TrackYourLife.Modules.GuitarTabs.Application.Features.GuitarTabDevices.Commands.UpdateGuitarTabDeviceSettings;

internal sealed class UpdateGuitarTabDeviceSettingsCommandValidator
    : AbstractValidator<UpdateGuitarTabDeviceSettingsCommand>
{
    public UpdateGuitarTabDeviceSettingsCommandValidator()
    {
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
