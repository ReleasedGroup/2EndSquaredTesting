using TestMining.Platform.Host.Auth;

namespace TestMining.Platform.Host.Navigation;

public static class ShellNavigationCatalog
{
    public static ShellNavigationItem Dashboard { get; } = new(
        "/dashboard",
        "Dashboard",
        "Shared operational view",
        "Review the current shell status, visible work areas, and role-specific affordances before moving into deeper workflows.",
        "Track workflow readiness",
        "Review-only overview",
        "This overview remains available to all authenticated roles.",
        "Review workflow shell",
        "Use this landing page to confirm the shell wiring before deeper features are implemented.",
        AppRoles.Review,
        AppRoles.Review,
        [
            "Role-specific navigation cards for each available workspace.",
            "A clear local-development authentication banner.",
            "An early shell suitable for local visual validation."
        ]);

    public static ShellNavigationItem Recordings { get; } = new(
        "/recordings",
        "Recordings",
        "Author workflow",
        "Start and monitor recording work once target allow-lists, browser defaults, and recording orchestration are in place.",
        "Launch recording flow",
        "Hidden from Viewer role",
        "Recording controls are limited to Author and Administrator personas.",
        "Start recording",
        "This placeholder marks where server-validated session setup and live capture status will appear.",
        AppRoles.Authoring,
        AppRoles.Authoring,
        [
            "Session setup and target URL entry placeholder.",
            "Recorder state and browser health placeholder.",
            "Live step stream region reserved for Sprint 2."
        ]);

    public static ShellNavigationItem Scenarios { get; } = new(
        "/scenarios",
        "Scenarios",
        "Review and authoring",
        "Inspect structured scenarios, version candidates, and approval-oriented editing workflows from the main authoring surface.",
        "Edit and review scenarios",
        "Read-only review remains available",
        "Viewer users can inspect scenario information without edit affordances.",
        "Open editor",
        "The scenario editor arrives in a later slice; for now this page reserves the workspace and role behavior.",
        AppRoles.Review,
        AppRoles.Authoring,
        [
            "Timeline and version-review workspace placeholder.",
            "Future locator, assertion, and data strategy panels.",
            "Role-aware edit affordance gating."
        ]);

    public static ShellNavigationItem Replays { get; } = new(
        "/replays",
        "Replays",
        "Diagnostics workspace",
        "Review replay execution results, step-level progress, and healing evidence once replay orchestration is delivered.",
        "Run replay from scenario",
        "Read-only diagnostics review",
        "Viewer users can inspect replay results but cannot start or rerun replay flows.",
        "Queue replay",
        "This shell reserves the future replay runner and diagnostics stream layout.",
        AppRoles.Review,
        AppRoles.Authoring,
        [
            "Replay launch panel placeholder.",
            "Step execution stream region for long-running updates.",
            "Diagnostics and healing evidence sidebar placeholder."
        ]);

    public static ShellNavigationItem Artefacts { get; } = new(
        "/artefacts",
        "Artefacts",
        "Generated outputs",
        "Review generated bundle metadata, manifests, and export details once deterministic generation lands.",
        "Preview bundle metadata",
        "Metadata-only review",
        "Sensitive export actions remain hidden unless the signed-in role can perform them.",
        "Open manifest",
        "This view keeps generated output review visible while export and download actions remain deferred.",
        AppRoles.Review,
        AppRoles.Authoring,
        [
            "Generated manifest and file inventory placeholder.",
            "Low-confidence warnings region for future generator output.",
            "Export summary area reserved for a later slice."
        ]);

    public static ShellNavigationItem Administration { get; } = new(
        "/administration",
        "Administration",
        "Policy and environment controls",
        "Manage environment allow-lists, retention rules, masking policy, and authentication bootstrap settings from a role-restricted area.",
        "Manage platform policy",
        "Restricted to Administrator",
        "Administration surfaces remain limited to Administrator because they change platform-wide policy.",
        "Configure environment",
        "This page reserves the admin surface for configuration, audit visibility, and policy controls.",
        [AppRoles.Administrator],
        [AppRoles.Administrator],
        [
            "Environment configuration and allow-list policy placeholder.",
            "Authentication bootstrap and retention controls placeholder.",
            "Audit-oriented admin workspace reserved for Sprint 1 follow-on slices."
        ]);

    public static IReadOnlyList<ShellNavigationItem> All { get; } =
    [
        Dashboard,
        Recordings,
        Scenarios,
        Replays,
        Artefacts,
        Administration
    ];
}
