document.addEventListener("DOMContentLoaded", function () {
    const inputs = document.querySelectorAll(".location-autocomplete");

    inputs.forEach(input => {
        let timeout = null;

        input.addEventListener("input", function () {
            const query = input.value.trim();
            if (query.length < 2) return;

            clearTimeout(timeout);
            timeout = setTimeout(() => {
                fetch(`/api/location/suggestions?input=${encodeURIComponent(query)}`)
                    .then(response => response.json())
                    .then(data => {
                        showSuggestions(input, data);
                    });
            }, 300);
        });
    });

    function showSuggestions(input, suggestions) {
        let list = input.nextElementSibling;
        if (!list || !list.classList.contains("autocomplete-list")) {
            list = document.createElement("div");
            list.classList.add("autocomplete-list", "list-group", "position-absolute", "shadow", "bg-white", "rounded");
            list.style.minWidth = input.offsetWidth + "px";
            list.style.top = input.offsetTop + input.offsetHeight + "px";
            list.style.left = input.offsetLeft + "px";
            list.style.zIndex = "1000";
            input.parentNode.appendChild(list);
        }

        list.innerHTML = "";
        suggestions.forEach(item => {
            const option = document.createElement("button");
            option.className = "list-group-item list-group-item-action";
            option.type = "button";
            option.style.whiteSpace = "nowrap";
            option.style.overflow = "hidden";
            option.style.textOverflow = "ellipsis";

            option.textContent = item;
            option.addEventListener("click", () => {
                input.value = item;
                list.innerHTML = "";
            });
            list.appendChild(option);
        });
    }
});
