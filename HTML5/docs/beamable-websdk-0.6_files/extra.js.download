// Per Version Consts
var VERSION_NAME = "" // "Unity", "Unreal", "WebSDK" or "" for all

if (window.location.pathname.includes('Unreal')) {
    VERSION_NAME = 'Unreal'
} else if (window.location.pathname.includes('Unity')) {
    VERSION_NAME = 'Unity'
} else if (window.location.pathname.includes('Web')) {
    VERSION_NAME = 'WebSDK'
}

// ---- Helpers ---------------------------------------------------------------
function WaitForVersionNav(selector, callback) {
    const interval = setInterval(() => {
        const element = document.querySelector(selector);
        if (element !== null) {
            clearInterval(interval);
            callback(element);
        }
    }, 50);
}

function insertHeader(container, label, beforeNode) {
    const header = document.createElement("div");
    header.className = "md-version__header";
    header.innerText = label;
    container.insertBefore(header, beforeNode);
}

function headerLabel(name) {
    const map = { Unity: "Unity SDK", Unreal: "Unreal SDK", WebSDK: "Web SDK" };
    return map[name] || name || "Versions";
}

function isVersionEntry(node) {
    // mike typically renders <a class="md-version__link">...</a> or <li>...</li>
    return node?.classList?.contains('md-version__link') || node.tagName === 'A' || node.tagName === 'LI';
}

// ---- Main ------------------------------------------------------------------
WaitForVersionNav('.md-version__list:not(.beam-sdk__list)', function(element) {
    const nodes = Array.from(element.children); // snapshot (avoid live-collection index shifts)

    // If VERSION_NAME is empty: show everything and add the three section headers like the original
    if (!VERSION_NAME) {
        // Find the first occurrence for each family BEFORE inserting anything
        const firstUnity  = nodes.find(n => isVersionEntry(n) && n.textContent.includes("Unity"));
        const firstUnreal = nodes.find(n => isVersionEntry(n) && n.textContent.includes("Unreal"));
        const firstWeb    = nodes.find(n => isVersionEntry(n) && n.textContent.includes("WebSDK"));
        const firstCli    = nodes.find(n => isVersionEntry(n) && n.textContent.includes("CLI"));

        if (firstUnity)  insertHeader(element, "Unity SDK",  firstUnity);
        if (firstUnreal) insertHeader(element, "Unreal SDK", firstUnreal);
        if (firstWeb)    insertHeader(element, "Web SDK",    firstWeb);
        if (firstCli)    insertHeader(element, "CLI",    firstWeb);
        return; // don't filter anything
    }

    // Otherwise: filter to only items that include VERSION_NAME
    const visible = [];
    for (const node of nodes) {
        if (!isVersionEntry(node)) continue;
        if (node.textContent.includes(VERSION_NAME)) {
            visible.push(node);
        } else {
            node.style.display = 'none';
            node.setAttribute('aria-hidden', 'true');
        }
    }

    if (visible.length > 0) {
        // insertHeader(element, headerLabel(VERSION_NAME), visible[0]);
    } else {
        const emptyMsg = document.createElement('div');
        emptyMsg.className = 'md-version__empty';
        emptyMsg.textContent = `No versions available for “${VERSION_NAME}”.`;
        element.prepend(emptyMsg);
    }

    // after filtering, strip off the sdk name, because it is extranous
    const subNodes = document.querySelectorAll('.md-header__topic .md-version *');
    const textReplacements = ['Unity-', 'Unreal-', 'WebSDK-', 'CLI-']
    for (var i = 0 ; i < subNodes.length; i ++){
        const subNode = subNodes[i];
        if (subNode.tagName == 'BUTTON' || subNode.tagName == 'A'){
            for (var j = 0 ; j < textReplacements.length; j ++){
                subNode.textContent = subNode.textContent.replace(textReplacements[j], '')
            }
        }
    }

});

WaitForVersionNav('.md-header__topic span.md-ellipsis', function(element) {
    const replacement = document.createElement('div');
    replacement.innerText = element.innerText;

    // add the class that gives us the drop-down style
    replacement.classList.add('md-version')
    replacement.classList.add('md-version__current')

    const list = document.createElement('ul');
    list.classList.add('md-version__list');
    list.classList.add('beam-sdk__list')
    replacement.appendChild(list);

    const dataEntries = [
        {
            name: 'Unity SDK',
            link: 'Unity-Latest'
        },
         {
            name: 'Unreal SDK',
            link: 'Unreal-Latest'
        },
         {
            name: 'Web SDK',
            link: 'WebSDK-Latest'
        },
        {
            name: 'CLI',
            link: 'CLI-Latest'
        }
    ]

    for (var i = 0 ; i < dataEntries.length; i ++){
        const data = dataEntries[i];

        const entry = document.createElement('li');
        entry.classList.add('md-version__item')

        const anchor = document.createElement('a');
        anchor.classList.add('md-version__link')
        entry.appendChild(anchor)
        anchor.innerText = data.name;
        anchor.setAttribute('href', '/' + data.link)

        list.appendChild(entry)
    }

    // swap out the element for the replacement.
    element.parentNode.insertBefore(replacement, element)
    element.parentNode.removeChild(element);
});