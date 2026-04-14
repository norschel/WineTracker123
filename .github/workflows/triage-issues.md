---
name: "AI Issue Triage"
on:
  issues:
    types: [opened, edited, reopened]
permissions:
  contents: read
safe-outputs:
  add-comment: null
  update-issue: null
  create-issue: null
  create-pull-request: null
concurrency:
  group: "ai-triage-issues"
  cancel-in-progress: false
---

# AI Issue Triage

This agentic workflow triages newly opened or updated issues: it classifies the issue type and priority, searches for duplicates, asks clarifying questions when the description is unclear, applies labels, and suggests or assigns the appropriate team member.

## Behavior / Instructions for the agent

- Trigger: run whenever an issue is opened, reopened, or edited.
- Goals (in priority order):
  1. Determine whether the issue is a duplicate of an existing issue. If duplicate: add a `duplicate` label and comment linking the original issue, then stop further triage.
  2. Classify issue type into one or more of: `bug`, `feature`, `docs`, `question`, `infra`, `performance`, `security`.
  3. Classify priority into one of: `priority/critical`, `priority/high`, `priority/medium`, `priority/low`.
  4. If the description is insufficient or ambiguous, post a polite clarifying comment with 2–4 specific questions and add a `needs-info` label. Do not assign until clarifying answers arrive.
  5. If clear and actionable, apply labels (`type/*`, `priority/*`, `area/*` where appropriate) and suggest or set an assignee based on keyword->owner mapping.

## Input data the agent MUST use

- Issue title and body
- Issue author and their previous activity (if any)
- Recent open issues and their titles/bodies (for duplicate detection)
- Repository CODEOWNERS or an owners mapping (if available) to pick assignees

## Duplicate detection rules

- Search open and recently-closed issues (last 12 months) for matching keywords and semantically similar titles/descriptions.
- If a likely duplicate is found (>= 80% similarity or clear identical symptom), comment: "This looks like a duplicate of #NN — linking for context" and add label `duplicate` and `triaged`.

## Clarity checks and clarifying questions

- Consider the description unclear when: body length < 50 chars, missing reproduction steps for bugs, no expected vs actual behavior, or ambiguous scope.
- When unclear, post a short checklist-style comment with targeted questions (e.g., "What steps reproduce the issue?", "What environment/OS/version did you use?").
- Add label `needs-info` and do NOT add an assignee until the author replies.

## Labeling and assignment rules

- Map keywords to labels and teams. Examples (agent should extend rules from repo metadata when available):
  - `crash`, `exception`, `stacktrace` -> label: `bug`; area: `runtime`; assign: runtime-maintainers
  - `ui`, `layout`, `accessibility` -> label: `bug` or `design`; assign: frontend-team
  - `api`, `endpoint`, `response` -> label: `bug` or `api`; assign: backend-team
  - `doc`, `readme`, `how-to` -> label: `docs`; assign: docs-team
  - `proposal`, `enhancement`, `improve` -> label: `feature`; assign: product-team
- If CODEOWNERS or a defined mapping is available, prefer that owner. If multiple owners, suggest a primary owner and leave a `triaged` label.

## Outputs (structured) — the agent should return exactly one JSON blob in a single comment or a set of GitHub API actions:

- labels: array of label names to add
- assignees: array of GitHub usernames to assign (may be empty)
- comment: text to post (e.g., duplicate link or clarifying questions)
- stop: boolean (true when no further triage needed, e.g., duplicate or needs-info)

## Example agent prompt (for the runtime)

You are an issue-triage assistant that can call the GitHub API. Follow the Behavior/Rules above exactly. For the incoming issue, do the following steps in order:

1. Search for likely duplicates across open & recently-closed issues. If duplicate, post a comment linking the canonical issue, add `duplicate` and `triaged` labels, and stop.
2. Classify type and priority using title/body cues and any explicit severity mentioned. Add `triaged` label after applying labels.
3. If description is short or missing key information, post a targeted clarifying comment and add `needs-info` label; set `stop=true`.
4. If clear, apply labels, and assign according to CODEOWNERS or the keyword->team mapping. Post a short comment summarizing the actions taken.

Return output as JSON in a single comment when actions are done (so actions are auditable). Make sure comments are friendly and include next steps for the reporter when asking for information.

## Safety and limits

- Do not make automatic assignments for security or legal issues; instead, add `security` label and mention `security-team` in a comment.
- Keep comments brief (< 400 words) and constructive.

## Usage / Deployment

This single-file workflow is intended for the gh-aw runner. After adding this file, compile and validate the workflow with the agentic workflows toolchain (`gh aw compile`) and run a dry-run against recent issues if supported.

---
## Usage Examples

- When an issue is clearly a bug with a stack trace, the agent will label `bug`, choose `priority/high` if crash or data loss is reported, and suggest an owner.
- When an issue is a duplicate, the agent will mark `duplicate` and link the previous issue.
