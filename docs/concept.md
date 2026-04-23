You can build this, and C# + Playwright is a solid stack for it.

The blunt truth is this: do not try to turn a raw click stream directly into final tests. That will produce brittle rubbish. Instead, record a user session, convert it into a semantic interaction model, then compile that into Playwright tests with resilience rules layered in. Playwright is a good fit because it already has strong locator strategies, auto-waiting, and test generation concepts you can borrow from rather than reinventing from scratch. Its generator prioritises role, text and test-id locators, and Playwright recommends using user-facing locators like GetByRole() for resilience. It also has auto-waiting and retryable assertions that reduce flakiness. 

What you are really building

Your app is not just a recorder. It is a test mining and stabilisation platform.

At a high level:

1. Capture what the app owner does in the browser.


2. Observe DOM, network, URL and state changes around each action.


3. Infer intent from each interaction.


4. Generalise selectors and assertions so they survive minor changes.


5. Generate readable C# Playwright tests.


6. Replay and heal when selectors or data drift slightly.



That middle bit, infer and generalise, is the whole game.

The architecture I would use

1. Desktop or web shell

Build a desktop shell in one of these ways:

Blazor Hybrid + .NET MAUI if you want a native-feeling app in C#

WPF if Windows-only is fine

ASP.NET Core + browser extension if you want the recorder to run mostly in-browser


For your use case, I would probably choose:

ASP.NET Core backend

Blazor Server or Blazor WebAssembly frontend

Playwright driver service in .NET

Browser extension or injected recorder script for DOM event capture


That gives you a nice interface and keeps the heavy lifting server-side.

2. Recorder engine

You need two channels of capture:

A. Browser instrumentation Inject JS into the target app to capture:

clicks

input changes

key presses

form submits

navigation

visible text near target

element attributes

accessibility role/name

DOM path

bounding box

surrounding labels

table/list context


B. Playwright observer At the same time, let Playwright monitor:

page URL changes

page loads

network requests/responses

dialogs

downloads

popup windows

element state before and after actions

screenshots and traces


Playwright already supports trace viewing and debugging workflows, which is useful as a reference point for your own session artefacts. 

3. Intermediate representation, not direct code

Store each recording as a structured model like this:

{
  "scenarioName": "Create customer",
  "steps": [
    {
      "type": "navigate",
      "url": "/customers"
    },
    {
      "type": "click",
      "target": {
        "role": "button",
        "name": "New Customer",
        "testId": "customer-new",
        "text": "New Customer",
        "cssFallback": ".toolbar button:nth-child(1)"
      },
      "context": {
        "pageTitle": "Customers",
        "nearbyHeading": "Customer List"
      }
    },
    {
      "type": "fill",
      "target": {
        "label": "First Name",
        "role": "textbox"
      },
      "value": "John"
    },
    {
      "type": "assert",
      "assertion": "toastVisible",
      "expected": {
        "containsText": "Customer created"
      }
    }
  ]
}

This is the key design move. Once you have this model, you can:

regenerate tests later

improve heuristics without rerecording

support multiple languages later

add AI-based healing without touching raw recordings


How to make it dynamic and not brittle

This is the hard part.

1. Use locator ranking, not one locator

For every recorded target, store a locator candidate set ranked best to worst.

Example:

1. test id


2. role + accessible name


3. label


4. text


5. placeholder


6. parent region + child role


7. stable attribute


8. CSS fallback


9. XPath only as last resort



That matches Playwright’s direction. It recommends prioritising user-facing attributes and explicit contracts such as role and test id. 

So your generated C# step should not just be:

await page.Locator("#btn123").ClickAsync();

It should be more like:

await LocatorResolver.ClickAsync(page, new LocatorSpec
{
    TestId = "customer-new",
    Role = AriaRole.Button,
    Name = "New Customer",
    Text = "New Customer"
});

Then LocatorResolver tries the candidates in order and validates uniqueness before acting.

2. Convert literal values into variable strategies

If a user types John Smith, that should not always become a hard-coded assertion.

Classify inputs as:

Test data: values the scenario truly depends on

Incidental data: values that can vary

Generated data: emails, IDs, dates, timestamps

Lookup data: chosen from a list


Instead of:

await Expect(page.GetByText("John Smith")).ToBeVisibleAsync();

generate:

await Expect(page.GetByRole(AriaRole.Row)
    .Filter(new() { HasTextRegex = new Regex("John") }))
    .ToBeVisibleAsync();

Or better, use scenario variables:

var customer = data.CustomerName;
await Expect(CustomerPage.RowFor(customer)).ToBeVisibleAsync();

3. Assert outcomes, not implementation details

Most brittle recorders assert the exact DOM after every click. That is madness.

You want to infer the likely user intention after each action:

clicked Save → assert save outcome, not button existence

filtered grid → assert rows reduced or filter chip appears

navigated to detail page → assert heading or URL pattern

submitted form → assert toast, redirect, or new record visible


Playwright’s web-first assertions and retrying behaviour help here because they wait for meaningful conditions instead of fixed sleeps. 

4. Use fuzzy matching for data-sensitive UI

You said: if data is slightly different, tests should still pass.

So add tolerance rules:

exact text becomes contains

IDs become regex

date/time values become parsers and range checks

tables become row existence checks by key columns

cards become existence by partial heading

counts can be >= 1 instead of exactly 7, unless count matters


Examples:

"Invoice 100234" → regex Invoice \d+

"23/04/2026 10:32" → date exists and is today

"Welcome, Nick" → contains "Welcome"


5. Detect repeating UI patterns

Your recorder should identify higher-order widgets:

grids

modals

tabs

dropdowns

autocomplete

toast notifications

date pickers

WYSIWYG editors


Then emit semantic actions:

Grid.SelectRowByText("Acme")

Modal.ClickPrimaryAction()

DatePicker.SelectDate(...)


rather than raw click coordinates or deep CSS selectors.

The components I would build

A. Session recorder

Responsibilities:

launch Playwright browser

navigate to app

inject recorder script

capture user actions and DOM snapshots

timestamp every event

correlate action to outcome


B. DOM intelligence service

This is your brains layer.

For each interacted element, compute:

role

accessible name

label

placeholder

inner text

stable attributes

visibility

uniqueness score

nearby landmarks like heading, form, dialog title


Then produce a ranked locator plan.

C. Scenario inference engine

Take the raw event stream and collapse noise:

remove hover/mousemove junk

merge click + input bursts

detect wait-for-navigation

infer submit/save/search/open/delete flows

detect retries by the human

infer assertions from visible changes


D. Test generator

Convert the scenario model into:

Playwright C# test classes

page objects or screen objects

data file or fixture objects

helper library for locator resolution and healing


E. Replay and healing engine

When a generated test fails:

inspect DOM again

try alternate locator candidates

compare accessible tree and nearby text

propose or auto-apply locator repairs

store the successful fix back to the scenario


This is where AI can help, but do not make AI the first line of execution. Use deterministic heuristics first, LLM only as a repair adviser.

Suggested C# solution structure

/src
  /Recorder.App -> Blazor or MAUI UI
  /Recorder.Api -> ASP.NET Core backend
  /Recorder.Playwright -> browser control and replay
  /Recorder.Capture -> event ingestion and DOM analysis
  /Recorder.Core -> models and interfaces
  /Recorder.Generation -> C# test code generator
  /Recorder.Healing -> locator healing logic
  /Recorder.Storage -> EF Core persistence
  /GeneratedTests -> emitted Playwright test project

Data model

Core tables/entities:

RecordingSession

Scenario

RecordedStep

ElementSnapshot

LocatorCandidate

AssertionCandidate

ReplayRun

HealingSuggestion


RecordedStep

Fields like:

Id

ScenarioId

Sequence

ActionType

UrlBefore

UrlAfter

Timestamp

RawValue

NormalisedValue

ScreenshotPath


ElementSnapshot

Fields like:

TagName

Role

AccessibleName

LabelText

Placeholder

TextContent

TestId

NameAttribute

CssPath

XPath

BoundingBox

ParentContextJson


LocatorCandidate

Fields like:

StrategyType

Expression

Priority

Confidence

WasUniqueAtRecordTime


How recording should work in practice

Step 1. Start a guided recording

User enters:

app URL

auth details or existing session

scenario name

optional environment

whether they want assertions suggested automatically


Step 2. User exercises the app

They use the app normally.

You record actions, but your UI also lets them mark:

“this matters”

“ignore this”

“this is an expected outcome”

“this value should be dynamic”

“this is sensitive, mask it”


That matters because not every user action should become a test step.

Step 3. Post-process the session

After recording, show a timeline:

Open Customers

Click New Customer

Enter customer details

Save

Verify success toast

Verify customer appears in grid


Let the user edit the scenario before generation.

Step 4. Generate code

Produce:

readable C# tests

helper methods

optional page objects

fixture data classes


What the generated C# might look like

using Microsoft.Playwright;
using Xunit;

public class CustomerTests : IAsyncLifetime
{
    private IPlaywright _playwright = default!;
    private IBrowser _browser = default!;
    private IBrowserContext _context = default!;
    private IPage _page = default!;

    public async Task InitializeAsync()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });
        _context = await _browser.NewContextAsync();
        _page = await _context.NewPageAsync();
    }

    [Fact]
    public async Task CreateCustomer()
    {
        var data = new CustomerData
        {
            FirstName = "John",
            LastName = "Smith",
            Email = $"john.{Guid.NewGuid():N}@example.com"
        };

        await _page.GotoAsync("https://app.example.com/customers");

        await LocatorResolver.ClickAsync(_page, new LocatorSpec
        {
            Role = AriaRole.Button,
            Name = "New Customer",
            TestId = "customer-new"
        });

        await LocatorResolver.FillAsync(_page, new LocatorSpec
        {
            Label = "First Name",
            Role = AriaRole.Textbox
        }, data.FirstName);

        await LocatorResolver.FillAsync(_page, new LocatorSpec
        {
            Label = "Last Name",
            Role = AriaRole.Textbox
        }, data.LastName);

        await LocatorResolver.FillAsync(_page, new LocatorSpec
        {
            Label = "Email",
            Role = AriaRole.Textbox
        }, data.Email);

        await LocatorResolver.ClickAsync(_page, new LocatorSpec
        {
            Role = AriaRole.Button,
            Name = "Save"
        });

        await Expect(_page.GetByRole(AriaRole.Status)).ToContainTextAsync("created");
        await Expect(_page.GetByRole(AriaRole.Row, new() { NameRegex = new System.Text.RegularExpressions.Regex(data.FirstName) }))
            .ToBeVisibleAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.CloseAsync();
        await _browser.CloseAsync();
        _playwright.Dispose();
    }
}

The key helper you need

Build a LocatorResolver that does this:

public static async Task<ILocator> ResolveAsync(IPage page, LocatorSpec spec)
{
    var candidates = new List<ILocator>();

    if (!string.IsNullOrWhiteSpace(spec.TestId))
        candidates.Add(page.GetByTestId(spec.TestId));

    if (spec.Role.HasValue && !string.IsNullOrWhiteSpace(spec.Name))
        candidates.Add(page.GetByRole(spec.Role.Value, new() { Name = spec.Name }));

    if (!string.IsNullOrWhiteSpace(spec.Label))
        candidates.Add(page.GetByLabel(spec.Label));

    if (!string.IsNullOrWhiteSpace(spec.Text))
        candidates.Add(page.GetByText(spec.Text));

    foreach (var locator in candidates)
    {
        if (await locator.CountAsync() == 1)
            return locator;
    }

    throw new InvalidOperationException($"Could not uniquely resolve locator for {spec}");
}

That is the minimum viable version. Later you add:

fuzzy name matching

scoped search within parent containers

nearby heading anchoring

healing suggestions


How to capture events from the browser

Inject a JS recorder with page.AddInitScriptAsync(...) so it starts before the app loads.

Capture events like:

click

input

change

submit

keydown


For each event, serialise:

event type

target metadata

value

page URL

active dialog or modal title

timestamp


Send it back with:

window.postMessage

console logs intercepted by Playwright

an exposed binding via Playwright


In .NET Playwright, exposed bindings/functions are a good bridge for sending browser-side events back into your app. Build your own recorder rather than trying to stretch Playwright’s built-in codegen too far, but absolutely learn from its locator-generation approach. Playwright’s codegen does analyse the page and recommend locators prioritising role, text and test id. 

How to infer assertions automatically

Use before-and-after snapshots around important actions.

After a click on Save, look for these signals in the next few seconds:

URL changes

toast appears

dialog closes

row count changes

page heading changes

a previously typed value appears in a list/detail view

success API response is returned


Then propose assertions ranked like:

1. success toast contains “saved”


2. URL matches /customers/{id}


3. heading contains created name


4. row containing new record visible



Let the user accept or reject them in the UI.

Nice UI ideas

Your interface should have four panes:

1. Recording pane

live browser preview

start/stop recording

pause

mark step as important

add note


2. Timeline pane

step-by-step session history

merge/delete/reorder actions

convert a step to assertion or data variable


3. Test view pane

generated C# preview

switch between raw steps and final test code

show healing confidence warnings


4. Replay pane

run test

see pass/fail

compare recorded element vs current element

approve healing fix


Where AI helps, and where it should not

Use AI for:

naming scenarios

summarising raw steps

suggesting assertions

classifying dynamic vs fixed data

proposing healed selectors when deterministic matching fails

refactoring generated code into page objects


Do not use AI for:

low-level event capture

core replay logic

primary element selection at runtime

timing and waiting logic


If you rely on AI every time the test runs, you will create slow, expensive, non-deterministic garbage.

What will make or break the product

Things that will work

apps with decent labels, roles, headings and test ids

standard CRUD business apps

predictable navigation and forms

teams willing to add testability hooks like data-testid


Things that will be painful

canvas-heavy UIs

virtualised tables

React components with poor accessibility

dynamic text everywhere

unstable DOM structure

apps using random generated IDs

drag-and-drop heavy workflows

rich text editors


Be prepared to add “testability recommendations” to the product:

add test ids

improve ARIA labels

expose stable row identifiers

add predictable success messages


Recommended build plan

Phase 1. Prove the core

Build:

recorder for clicks, fills, selects, navs

timeline UI

locator ranking

C# Playwright code generation

basic replay


Only support:

buttons

textboxes

checkboxes

links

dropdowns

basic grids

toasts


Phase 2. Make it robust

Add:

assertion inference

data variable extraction

regex and partial matching

scoped locators

replay diagnostics

screenshots per step


Phase 3. Make it product-worthy

Add:

login/session management

page object generation

CI export

trace viewer

healing workflow

collaboration and scenario versioning


Phase 4. Add AI carefully

Add:

scenario summaries

assertion suggestions

healing proposal review

test refactoring suggestions


My opinion on stack choices

If you want the most practical build:

Backend: ASP.NET Core

Frontend: Blazor Server

Database: PostgreSQL or SQL Server

Browser automation: Microsoft.Playwright for .NET

Code generation: Roslyn or Scriban templates

Desktop packaging later: .NET MAUI Hybrid or Electron wrapper if needed


Why not just use Playwright codegen directly? Because it is a great starting point, but your product needs more than generated clicks. Playwright codegen is designed to help you quickly generate tests from interactions, and it improves locators to be more resilient, but it is not a full “record a business owner and convert that into maintainable enterprise scenarios” product by itself. 

The single most important design decision

Store intent-rich structured scenarios, not just emitted code.

If you get that right, everything else gets easier:

better regeneration

language portability

self-healing

nicer UI

maintainable tests


If you get that wrong, you will spend your life cleaning up brittle selectors and flaky recordings.

My recommendation in one sentence

Build a C# app that records browser interactions into a semantic scenario model, ranks resilient Playwright locators, infers business-level assertions, and generates editable Playwright C# tests with replay and healing.