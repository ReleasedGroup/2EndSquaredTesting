const body = document.body;

document.addEventListener("DOMContentLoaded", () => {
    wireSharedStatus();

    switch (body.dataset.page) {
        case "home":
            wireHome();
            break;
        case "forms":
            wireForms();
            break;
        case "grid":
            wireGrid();
            break;
        case "modal":
            wireModal();
            break;
        case "login":
            wireLogin();
            break;
    }
});

async function requestJson(url, options) {
    const response = await fetch(url, {
        headers: { "Content-Type": "application/json" },
        credentials: "same-origin",
        ...options
    });

    if (!response.ok) {
        throw new Error(`Request failed with ${response.status}`);
    }

    return await response.json();
}

function showToast(message) {
    const stack = document.getElementById("toast-stack");
    const toast = document.createElement("div");
    toast.className = "toast";
    toast.textContent = message;
    stack.appendChild(toast);
    window.setTimeout(() => toast.remove(), 2800);
}

async function wireSharedStatus() {
    const node = document.getElementById("shared-status");
    if (!node) {
        return;
    }

    const session = await requestJson("/api/session");
    node.textContent = session.authenticated
        ? `Logged in as ${session.userName}`
        : "Anonymous fixture visitor";
}

async function wireHome() {
    const node = document.getElementById("customer-count");
    const state = await requestJson("/api/test/state");
    node.textContent = `${state.customers.length} seeded records`;
}

async function wireForms() {
    const form = document.getElementById("customer-form");
    const result = document.getElementById("form-result");
    const resetButton = document.getElementById("forms-reset");

    for (const input of form.querySelectorAll("input, select")) {
        input.id = `${input.name}-${crypto.randomUUID().slice(0, 8)}`;
        const label = form.querySelector(`label[data-for='${input.name}']`);
        label?.setAttribute("for", input.id);
    }

    form.addEventListener("submit", async event => {
        event.preventDefault();
        const payload = Object.fromEntries(new FormData(form).entries());
        const state = await requestJson("/api/customers", {
            method: "POST",
            body: JSON.stringify(payload)
        });

        result.textContent = `Saved ${payload.name}. Customer count is now ${state.customers.length}.`;
        showToast("Customer saved and toast flow triggered.");
        form.reset();
    });

    resetButton.addEventListener("click", async () => {
        await requestJson("/api/test/reset", { method: "POST" });
        result.textContent = "Fixture state reset to deterministic seed data.";
        showToast("Fixture data reset.");
    });
}

async function wireGrid() {
    const tableBody = document.getElementById("grid-body");
    const refreshButton = document.getElementById("grid-refresh");
    const resetButton = document.getElementById("grid-reset");

    async function render() {
        const state = await requestJson("/api/test/state");
        tableBody.innerHTML = "";

        for (const customer of state.customers) {
            const row = document.createElement("tr");
            row.innerHTML = `
                <td>${customer.name}</td>
                <td>${customer.email}</td>
                <td>${customer.role}</td>
                <td>${customer.isActive ? "Active" : "Inactive"}</td>
                <td>
                    <button type="button" data-edit="${customer.id}">Edit</button>
                    <button type="button" class="secondary" data-delete="${customer.id}">Delete</button>
                </td>`;
            tableBody.appendChild(row);
        }
    }

    tableBody.addEventListener("click", async event => {
        const target = event.target;
        if (!(target instanceof HTMLButtonElement)) {
            return;
        }

        const editId = target.dataset.edit;
        const deleteId = target.dataset.delete;
        if (editId) {
            const state = await requestJson("/api/test/state");
            const customer = state.customers.find(item => item.id === Number(editId));
            const name = window.prompt("Updated name", customer.name);
            if (!name) {
                return;
            }

            await requestJson(`/api/customers/${editId}`, {
                method: "PUT",
                body: JSON.stringify({
                    name,
                    email: customer.email,
                    role: customer.role,
                    isActive: customer.isActive
                })
            });

            showToast("Grid row edited.");
            await render();
        }

        if (deleteId) {
            await requestJson(`/api/customers/${deleteId}`, { method: "DELETE" });
            showToast("Grid row deleted.");
            await render();
        }
    });

    refreshButton.addEventListener("click", render);
    resetButton.addEventListener("click", async () => {
        await requestJson("/api/test/reset", { method: "POST" });
        showToast("Grid data reset.");
        await render();
    });

    await render();
}

function wireModal() {
    const openButton = document.getElementById("open-modal");
    const closeButton = document.getElementById("close-modal");
    const submitButton = document.getElementById("save-modal");
    const backdrop = document.getElementById("dialog-backdrop");
    const auditList = document.getElementById("audit-trail");

    async function renderAuditTrail() {
        const state = await requestJson("/api/test/state");
        auditList.innerHTML = "";
        for (const entry of state.auditTrail.slice().reverse()) {
            const item = document.createElement("li");
            item.textContent = entry;
            auditList.appendChild(item);
        }
    }

    openButton.addEventListener("click", () => backdrop.classList.add("visible"));
    closeButton.addEventListener("click", () => backdrop.classList.remove("visible"));
    submitButton.addEventListener("click", async () => {
        const notes = document.getElementById("modal-notes").value.trim() || "Saved modal review.";
        await requestJson("/api/audit-events", {
            method: "POST",
            body: JSON.stringify({ message: notes })
        });
        backdrop.classList.remove("visible");
        showToast("Modal workflow completed.");
        await renderAuditTrail();
    });

    void renderAuditTrail();
}

function wireLogin() {
    const form = document.getElementById("login-form");
    const logoutButton = document.getElementById("logout-button");
    const result = document.getElementById("login-result");

    async function renderSession() {
        const session = await requestJson("/api/session");
        result.textContent = session.authenticated
            ? `Authenticated as ${session.userName}.`
            : "Not authenticated.";
    }

    form.addEventListener("submit", async event => {
        event.preventDefault();
        const payload = Object.fromEntries(new FormData(form).entries());
        try {
            await requestJson("/api/login", {
                method: "POST",
                body: JSON.stringify(payload)
            });
            showToast("Fixture login succeeded.");
        } catch {
            showToast("Fixture login failed.");
        }

        await renderSession();
    });

    logoutButton.addEventListener("click", async () => {
        await requestJson("/api/logout", { method: "POST" });
        showToast("Fixture logout succeeded.");
        await renderSession();
    });

    void renderSession();
}
