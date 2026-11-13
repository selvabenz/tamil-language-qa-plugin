# Paratext compatibility plan

## Target

| Item | Value |
| --- | --- |
| Host | Paratext 9.5.110.1 |
| Platform | Windows |
| Operation | Offline scope scanning, verse-by-verse review, and explicit safe application |
| Current compatibility status | Compiles against installed SDK 2.0.100; installed and host context menu discovered; full host acceptance pending |

Paratext's official download page lists the requested version. Its plugin documentation describes Plugin API 2.0 introduced with Paratext 9.2, and the linked demo repository provides integration examples. These establish research starting points; they do not prove this project's compatibility with the target build.

- [Official Paratext downloads](https://paratext.org/download/)
- [Paratext Plugin API introduction](https://paratext.org/features/whats-new/whats-new-in-paratext-9-2/)
- [Paratext demo plugins](https://github.com/ubsicap/paratext_demo_plugins)

## Before choosing the implementation framework

Inspect the installed host and supported SDK documentation to verify:

1. Required .NET target, supported CPU architecture, API assembly versions, and plugin entry points.
2. Plugin discovery and installation paths, deployment metadata, dependency loading, and update behavior.
3. Supported project text access, verse navigation, UI threading, and cancellation.
4. SDK redistribution permissions; reference host assemblies without publishing proprietary binaries.

Keep the checking core independent of host APIs. Use a thin Paratext adapter for text access and review navigation. Bundle all licensed runtime resources needed to scan with networking disabled. Select the final implementation language after confirming SDK requirements.

## Compatibility evidence required

A release must record a successful build and a manual run on the exact target host. Exercise plugin discovery, scope selection, read-only scanning, Tamil display, verse navigation, cancellation, exception handling, and offline operation. Confirm source text is unchanged after scanning. Verify native-style window behavior and inline annotation/context-menu capabilities, chapter-level writes under host locks, permissions, stale edits, and guarded recovery on copied projects. Confirm build identity appears in the UI. Until this evidence exists, describe Paratext 9.5.110.1 as a target rather than supported compatibility.
