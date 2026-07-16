# ColonyCopilot

ColonyCopilot was an April 2024 experiment in connecting agentic AI to an existing video game surface.

The idea was to go beyond putting a chat window next to RimWorld. The mod collected live information about a colony, passed that context into an OpenAI Assistant, and exposed game actions as tools the model could call. The agent could inspect colonists and resources, respond through an in-game interface and text-to-speech, and request actions such as mining, construction, and tree cutting.

It went quite well as an experiment. The prototype demonstrated that an agentic loop could observe an existing game, reason about its state, and act through the game's own systems without the game having been designed for AI control.

## Why it stopped

Most of the implementation in this repository was written between April 19 and April 30, 2024. OpenAI had introduced the [Assistants API](https://openai.com/index/new-models-and-developer-products-announced-at-devday/) about five months earlier, alongside GPT-4 Turbo.

At the time, GPT-4 Turbo was the practical frontier model for this kind of tool-using loop. It cost $10 per million input tokens and $30 per million output tokens. ColonyCopilot repeatedly sent a growing colony context, waited for runs, executed requested tools, returned their results, and sometimes generated speech. That was workable for a prototype, but prohibitively expensive for something intended to run continuously during normal play.

The economics changed soon afterward. [GPT-4o launched on May 13, 2024](https://openai.com/index/hello-gpt-4o/) at half the API price of GPT-4 Turbo, less than two weeks after the last substantive ColonyCopilot commit. [GPT-4o mini followed on July 18, 2024](https://openai.com/index/gpt-4o-mini-advancing-cost-efficient-intelligence/) at $0.15 per million input tokens and $0.60 per million output tokens. The cost problem was real during the project's active development window, but the model market moved quickly after it.

The Assistants API used here has since been deprecated in favor of OpenAI's Responses API. This code is preserved as a working experiment from that period, not as a current integration example or a finished mod.

I came back to the idea in late 2025 with [Artificial Rimtelligence](https://github.com/Void-n-Null/Artificial-Rimtelligence), a more ambitious multi-mod take on AI colony management that stopped at early scaffolding. ColonyCopilot remains the version that actually observed a colony and acted in it.

## Repository state

The project is unfinished and is not packaged for installation. The repository contains the custom OpenAI client, assistant and tool-call loop, RimWorld integration, colony-context collection, in-game actions, UI, text-to-speech support, and local tests for the tool-function layer.

## Building

Building requires RimWorld 1.5 managed assemblies, a compatible `0Harmony.dll`, .NET Framework 4.7.2 targeting support, and NuGet package restore. Newtonsoft.Json and the remaining managed dependencies are restored from NuGet rather than committed to the repository.

Set these environment variables before building, or provide equivalent MSBuild properties:

```text
RIMWORLD_MANAGED_DIR=C:\path\to\RimWorldWin64_Data\Managed
HARMONY_ASSEMBLY_PATH=C:\path\to\0Harmony.dll
```

The repository does not contain compiled assemblies or RimWorld and Unity binaries. Build output is generated locally under `Assemblies/` and is ignored by Git.

## License

This source and its original visual assets are published for reference with all rights reserved. See `LICENSE` for details. Third-party names, trademarks, game assets, and dependencies remain the property of their respective owners.
