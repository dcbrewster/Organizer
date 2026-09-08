using System.ComponentModel;
using System.Reflection;
using System.Runtime.ExceptionServices;
using CleanOrganizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CleanOrganizer.Tests;

[TestClass]
public sealed class ContactGridBindingTests
{
    [TestMethod]
    public void AddingContactThroughBoundListKeepsTheAddedContact()
    {
        RunOnStaThread(() =>
        {
            var contacts = new List<Contact>();
            var bindingList = new BindingList<Contact>(contacts);
            using var bindingSource = new BindingSource { DataSource = bindingList };
            using var grid = new DataGridView
            {
                AutoGenerateColumns = true,
                DataSource = bindingSource,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false
            };

            var contact = new Contact
            {
                FirstName = "Ada",
                LastName = "Lovelace",
                Email = "ada@example.test"
            };

            bindingList.Add(contact);
            InvokeSyncList(contacts, bindingList);

            Assert.AreEqual(1, contacts.Count, "The contact should remain after syncing the bound list back to the source list.");
            Assert.AreSame(contact, contacts[0]);
        });
    }

    private static void InvokeSyncList<T>(List<T> target, BindingList<T> source)
    {
        var method = typeof(MainForm).GetMethod("SyncList", BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new MissingMethodException(nameof(MainForm), "SyncList");

        try
        {
            method.MakeGenericMethod(typeof(T)).Invoke(null, [target, source]);
        }
        catch (TargetInvocationException exception) when (exception.InnerException is not null)
        {
            ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
        }
    }

    private static void RunOnStaThread(Action action)
    {
        Exception? exception = null;
        var thread = new Thread(() =>
        {
            try
            {
                action();
            }
            catch (Exception caught)
            {
                exception = caught;
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join();

        if (exception is not null)
        {
            ExceptionDispatchInfo.Capture(exception).Throw();
        }
    }
}
