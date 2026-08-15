(function () {
    "use strict";

    function formatCurrency(value) {
        return Number(value).toLocaleString(
            "he-IL",
            {
                minimumFractionDigits: 2,
                maximumFractionDigits: 2
            })
            + " ₪";
    }

    function formatPercentage(value) {
        return Math.abs(Number(value)).toLocaleString(
            "he-IL",
            {
                minimumFractionDigits: 1,
                maximumFractionDigits: 1
            })
            + "%";
    }

    function updatePointComparison(
        chartSection,
        chartPoints,
        selectedPointIndexes) {

        var comparisonText =
            chartSection.querySelector(
                ".product-cost-point-comparison-text");

        var comparisonResult =
            chartSection.querySelector(
                ".product-cost-point-comparison-result");

        if (!comparisonText || !comparisonResult) {
            return;
        }

        comparisonResult.classList.remove(
            "text-danger",
            "text-success",
            "text-secondary");

        if (selectedPointIndexes.length === 1) {
            var selectedPoint =
                chartPoints[selectedPointIndexes[0]];

            comparisonText.textContent =
                "נבחרה נקודה: "
                + selectedPoint.label
                + " — "
                + formatCurrency(selectedPoint.value)
                + ". בחר נקודה נוספת.";

            comparisonResult.textContent = "";
            return;
        }

        var orderedIndexes =
            selectedPointIndexes
                .slice()
                .sort(function (firstIndex, secondIndex) {
                    return firstIndex - secondIndex;
                });

        var startPoint = chartPoints[orderedIndexes[0]];
        var endPoint = chartPoints[orderedIndexes[1]];
        var startValue = Number(startPoint.value);
        var endValue = Number(endPoint.value);

        comparisonText.textContent =
            "מ־"
            + startPoint.label
            + " ("
            + formatCurrency(startValue)
            + ") עד "
            + endPoint.label
            + " ("
            + formatCurrency(endValue)
            + "):";

        if (startValue === 0) {
            comparisonResult.textContent =
                "לא ניתן לחשב אחוז מעלות התחלתית של 0";

            comparisonResult.classList.add(
                "text-secondary");

            return;
        }

        var changePercentage =
            (endValue - startValue)
            / startValue
            * 100;

        if (changePercentage > 0) {
            comparisonResult.textContent =
                "↑ " + formatPercentage(changePercentage);

            comparisonResult.classList.add("text-danger");
            return;
        }

        if (changePercentage < 0) {
            comparisonResult.textContent =
                "↓ " + formatPercentage(changePercentage);

            comparisonResult.classList.add("text-success");
            return;
        }

        comparisonResult.textContent = "ללא שינוי 0.0%";
        comparisonResult.classList.add("text-secondary");
    }

    function createProductCostChart(canvas) {
        if (!canvas
            || canvas.dataset.chartInitialized === "true"
            || typeof Chart === "undefined") {
            return;
        }

        var chartSection = canvas.closest("section");

        if (!chartSection) {
            return;
        }

        var dataInput = chartSection.querySelector(
            ".product-cost-chart-data");

        if (!dataInput) {
            return;
        }

        var chartPoints;

        try {
            chartPoints = JSON.parse(dataInput.value);
        }
        catch (error) {
            return;
        }

        if (!Array.isArray(chartPoints)
            || chartPoints.length === 0) {
            return;
        }

        var labels = chartPoints.map(function (point) {
            return String(point.label);
        });

        var values = chartPoints.map(function (point) {
            return Number(point.value);
        });

        var productName =
            canvas.getAttribute("data-product-name")
            || "מוצר";

        var rootStyles =
            window.getComputedStyle(
                document.documentElement);

        var primaryColor =
            rootStyles
                .getPropertyValue("--bs-primary")
                .trim()
            || "#0d6efd";

        var selectionColor =
            rootStyles
                .getPropertyValue("--bs-warning")
                .trim()
            || "#ffc107";

        var selectedPointIndexes = [];

        new Chart(
            canvas,
            {
                type: "line",

                data: {
                    labels: labels,

                    datasets: [
                        {
                            label:
                                "עלות כוללת - "
                                + productName,

                            data: values,
                            borderColor: primaryColor,
                            backgroundColor: primaryColor,
                            borderWidth: 2,
                            pointRadius: 4,
                            pointHoverRadius: 6,
                            pointHitRadius: 12,
                            tension: 0.2,
                            fill: false
                        }
                    ]
                },

                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    locale: "he-IL",

                    interaction: {
                        mode: "index",
                        intersect: true
                    },

                    onClick: function (event, activeElements, chart) {
                        if (!activeElements
                            || activeElements.length === 0) {
                            return;
                        }

                        var clickedPointIndex =
                            activeElements[0].index;

                        if (selectedPointIndexes.length === 2) {
                            selectedPointIndexes = [];
                        }

                        if (selectedPointIndexes.length === 1
                            && selectedPointIndexes[0]
                            === clickedPointIndex) {
                            return;
                        }

                        selectedPointIndexes.push(clickedPointIndex);

                        var dataset = chart.data.datasets[0];

                        dataset.pointRadius =
                            values.map(function (value, index) {
                                return selectedPointIndexes
                                    .indexOf(index) >= 0
                                    ? 8
                                    : 4;
                            });

                        dataset.pointBackgroundColor =
                            values.map(function (value, index) {
                                return selectedPointIndexes
                                    .indexOf(index) >= 0
                                    ? selectionColor
                                    : primaryColor;
                            });

                        dataset.pointBorderColor =
                            values.map(function (value, index) {
                                return selectedPointIndexes
                                    .indexOf(index) >= 0
                                    ? "#212529"
                                    : primaryColor;
                            });

                        dataset.pointBorderWidth =
                            values.map(function (value, index) {
                                return selectedPointIndexes
                                    .indexOf(index) >= 0
                                    ? 2
                                    : 1;
                            });

                        chart.update();

                        updatePointComparison(
                            chartSection,
                            chartPoints,
                            selectedPointIndexes);
                    },

                    plugins: {
                        legend: {
                            display: false
                        },

                        tooltip: {
                            rtl: true,
                            textDirection: "rtl",

                            callbacks: {
                                label: function (context) {
                                    return "עלות כוללת: "
                                        + formatCurrency(
                                            context.parsed.y);
                                }
                            }
                        }
                    },

                    scales: {
                        x: {
                            title: {
                                display: true,
                                text: "מועד החישוב"
                            }
                        },

                        y: {
                            beginAtZero: true,

                            title: {
                                display: true,
                                text: "עלות כוללת (₪)"
                            },

                            ticks: {
                                callback: function (value) {
                                    return formatCurrency(value);
                                }
                            }
                        }
                    }
                }
            });

        canvas.dataset.chartInitialized = "true";
    }

    function initializeChartsInside(container) {
        var canvases =
            container.querySelectorAll(
                ".product-cost-history-chart");

        canvases.forEach(function (canvas) {
            createProductCostChart(canvas);
        });
    }

    document.addEventListener(
        "DOMContentLoaded",
        function () {
            var openProductGroups =
                document.querySelectorAll(
                    ".collapse.show");

            openProductGroups.forEach(
                function (group) {
                    initializeChartsInside(group);
                });
        });

    document.addEventListener(
        "shown.bs.collapse",
        function (event) {
            initializeChartsInside(event.target);
        });
})();