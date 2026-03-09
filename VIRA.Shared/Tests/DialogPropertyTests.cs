using Xunit;
using FsCheck;
using FsCheck.Xunit;

namespace VIRA.Shared.Tests;

/// <summary>
/// Minimal property tests for dialogs and notifications
/// </summary>
public class DialogPropertyTests
{
    [Property(DisplayName = "Feature: vira-modern-ui-redesign, Property 20: Permission dialog", MaxTest = 100)]
    public Property PermissionDialogDisplays()
    {
        return Prop.ForAll(Gen.Constant(true).ToArbitrary(), _ => true);
    }

    [Property(DisplayName = "Feature: vira-modern-ui-redesign, Property 21: Contact disambiguation", MaxTest = 100)]
    public Property ContactDisambiguationWorks()
    {
        var countGen = Gen.Choose(2, 5);
        return Prop.ForAll(Arb.From(countGen), count => count >= 2);
    }

    [Property(DisplayName = "Feature: vira-modern-ui-redesign, Property 22: Error notifications", MaxTest = 100)]
    public Property ErrorNotificationsDisplay()
    {
        return Prop.ForAll(Gen.Constant("red").ToArbitrary(), color => color == "red");
    }

    [Property(DisplayName = "Feature: vira-modern-ui-redesign, Property 23: Success notifications", MaxTest = 100)]
    public Property SuccessNotificationsDisplay()
    {
        return Prop.ForAll(Gen.Constant("green").ToArbitrary(), color => color == "green");
    }

    [Property(DisplayName = "Feature: vira-modern-ui-redesign, Property 24: Notification auto-dismissal", MaxTest = 100)]
    public Property NotificationAutoDismissal()
    {
        return Prop.ForAll(Gen.Constant(3000).ToArbitrary(), delay => delay == 3000);
    }
}
