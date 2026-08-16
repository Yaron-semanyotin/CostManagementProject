(function () {
    "use strict";

    var charts = [];

    function formatCurrency(value) {
        return Number(value).toLocaleString("he-IL", {
            minimumFractionDigits: 2,
            maximumFractionDigits: 2
        }) + " ₪";
    }

    function formatPercentage(value) {
        return Math.abs(Number(value)).toLocaleString("he-IL", {
            minimumFractionDigits: 1,
            maximumFractionDigits: 1
        }) + "%";
    }

    function getCssVariable(name, fallback) {
        var value = window.getComputedStyle(document.documentElement)
            .getPropertyValue(name).trim();

        return value || fallback;
    }

    function getThemeColors() {
        return {
            primary: getCssVariable("--cw-primary", "#2563EB"),
            warning: getCssVariable("--cw-warning", "#B45309"),
            border: getCssVariable("--cw-border", "#E2E8F0"),
            text: getCssVariable("--cw-text-primary", "#0F172A"),
            secondary: getCssVariable("--cw-text-secondary", "#64748B"),
            surface: getCssVariable("--cw-surface", "#FFFFFF")
        };
    }

    function updatePointComparison(section, points, selectedIndexes) {
        var text = section.querySelector(".product-cost-point-comparison-text");
        var result = section.querySelector(".product-cost-point-comparison-result");

        if (!text || !result) {
            return;
        }

        result.classList.remove("text-danger", "text-success", "text-secondary");

        if (selectedIndexes.length === 1) {
            var selected = points[selectedIndexes[0]];

            text.textContent = "נבחרה נקודה: " + selected.label + " — "
                + formatCurrency(selected.value) + ". בחר נקודה נוספת.";
            result.textContent = "";
            return;
        }

        var indexes = selectedIndexes.slice().sort(function (first, second) {
            return first - second;
        });

        var startPoint = points[indexes[0]];
        var endPoint = points[indexes[1]];
        var startValue = Number(startPoint.value);
        var endValue = Number(endPoint.value);

        text.textContent = "מ־" + startPoint.label + " (" + formatCurrency(startValue)
            + ") עד " + endPoint.label + " (" + formatCurrency(endValue) + "):";

        if (startValue === 0) {
            result.textContent = "לא ניתן לחשב אחוז מעלות התחלתית של 0";
            result.classList.add("text-secondary");
            return;
        }

        var percentage = (endValue - startValue) / startValue * 100;

        if (percentage > 0) {
            result.textContent = "↑ " + formatPercentage(percentage) + " עלייה בעלות";
            result.classList.add("text-danger");
            return;
        }

        if (percentage < 0) {
            result.textContent = "↓ " + formatPercentage(percentage) + " ירידה בעלות";
            result.classList.add("text-success");
            return;
        }

        result.textContent = "ללא שינוי 0.0%";
        result.classList.add("text-secondary");
    }

    function applyChartTheme(chart) {
        var colors = getThemeColors();
        var dataset = chart.data.datasets[0];
        var values = chart.$costWiseValues;
        var selectedIndexes = chart.$costWiseSelectedIndexes;

        dataset.borderColor = colors.primary;
        dataset.backgroundColor = colors.primary;

        dataset.pointRadius = values.map(function (value, index) {
            return selectedIndexes.indexOf(index) >= 0 ? 7 : 3;
        });

        dataset.pointBackgroundColor = values.map(function (value, index) {
            return selectedIndexes.indexOf(index) >= 0
                ? colors.warning
                : colors.primary;
        });

        dataset.pointBorderColor = values.map(function (value, index) {
            return selectedIndexes.indexOf(index) >= 0
                ? colors.surface
                : colors.primary;
        });

        chart.options.scales.x.grid.color = colors.border;
        chart.options.scales.y.grid.color = colors.border;
        chart.options.scales.x.ticks.color = colors.secondary;
        chart.options.scales.y.ticks.color = colors.secondary;
        chart.options.scales.x.title.color = colors.secondary;
        chart.options.scales.y.title.color = colors.secondary;
        chart.options.plugins.tooltip.backgroundColor = colors.surface;
        chart.options.plugins.tooltip.titleColor = colors.text;
        chart.options.plugins.tooltip.bodyColor = colors.text;
        chart.options.plugins.tooltip.borderColor = colors.border;
        chart.update("none");
    }

    function createProductCostChart(canvas) {
        if (!canvas || canvas.dataset.chartInitialized === "true"
            || typeof Chart === "undefined") {
            return;
        }

        var section = canvas.closest("section");
        var input = section
            ? section.querySelector(".product-cost-chart-data")
            : null;

        if (!section || !input) {
            return;
        }

        var points;

        try {
            points = JSON.parse(input.value);
        }
        catch (error) {
            return;
        }

        if (!Array.isArray(points) || points.length === 0) {
            return;
        }

        var labels = points.map(function (point) {
            return String(point.label);
        });

        var values = points.map(function (point) {
            return Number(point.value);
        });

        var productName = canvas.getAttribute("data-product-name") || "מוצר";
        var colors = getThemeColors();
        var selectedIndexes = [];

        var chart = new Chart(canvas, {
            type: "line",
            data: {
                labels: labels,
                datasets: [{
                    label: "עלות כוללת - " + productName,
                    data: values,
                    borderColor: colors.primary,
                    backgroundColor: colors.primary,
                    borderWidth: 2,
                    pointRadius: 3,
                    pointHoverRadius: 6,
                    pointHitRadius: 12,
                    tension: 0,
                    fill: false
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                locale: "he-IL",
                interaction: {
                    mode: "index",
                    intersect: false
                },
                onClick: function (event, activeElements, currentChart) {
                    if (!activeElements || activeElements.length === 0) {
                        return;
                    }

                    var index = activeElements[0].index;

                    if (selectedIndexes.length === 2) {
                        selectedIndexes.length = 0;
                    }

                    if (selectedIndexes.length === 1 && selectedIndexes[0] === index) {
                        return;
                    }

                    selectedIndexes.push(index);
                    applyChartTheme(currentChart);
                    updatePointComparison(section, points, selectedIndexes);
                },
                plugins: {
                    legend: {
                        display: false
                    },
                    tooltip: {
                        rtl: true,
                        textDirection: "rtl",
                        borderWidth: 1,
                        callbacks: {
                            title: function (items) {
                                return items.length > 0 ? items[0].label : "";
                            },
                            label: function (context) {
                                return "עלות כוללת: " + formatCurrency(context.parsed.y);
                            }
                        }
                    }
                },
                scales: {
                    x: {
                        grid: {
                            color: colors.border
                        },
                        ticks: {
                            color: colors.secondary
                        },
                        title: {
                            display: true,
                            text: "מועד החישוב",
                            color: colors.secondary
                        }
                    },
                    y: {
                        beginAtZero: true,
                        grid: {
                            color: colors.border
                        },
                        ticks: {
                            color: colors.secondary,
                            callback: function (value) {
                                return formatCurrency(value);
                            }
                        },
                        title: {
                            display: true,
                            text: "עלות כוללת (₪)",
                            color: colors.secondary
                        }
                    }
                }
            }
        });

        chart.$costWiseValues = values;
        chart.$costWiseSelectedIndexes = selectedIndexes;
        charts.push(chart);
        canvas.dataset.chartInitialized = "true";
        applyChartTheme(chart);
    }

    function initializeChartsInside(container) {
        container.querySelectorAll(".product-cost-history-chart")
            .forEach(function (canvas) {
                createProductCostChart(canvas);
            });
    }

    document.addEventListener("DOMContentLoaded", function () {
        document.querySelectorAll(".collapse.show").forEach(function (group) {
            initializeChartsInside(group);
        });
    });

    document.addEventListener("shown.bs.collapse", function (event) {
        initializeChartsInside(event.target);
    });

    document.addEventListener("costwise:themechange", function () {
        charts.forEach(function (chart) {
            applyChartTheme(chart);
        });
    });
})();