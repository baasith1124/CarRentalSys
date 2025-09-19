// Enhanced Interactive Charts for Dashboard

class DashboardCharts {
    constructor() {
        this.charts = {};
        this.chartOptions = this.getDefaultChartOptions();
    }

    getDefaultChartOptions() {
        return {
            responsive: true,
            maintainAspectRatio: false,
            interaction: {
                intersect: false,
                mode: 'index'
            },
            animation: {
                duration: 2000,
                easing: 'easeOutCubic'
            },
            plugins: {
                legend: {
                    position: 'bottom',
                    labels: {
                        padding: 20,
                        usePointStyle: true,
                        font: {
                            size: 12,
                            family: 'Segoe UI, sans-serif'
                        }
                    }
                },
                tooltip: {
                    backgroundColor: 'rgba(0, 0, 0, 0.9)',
                    titleColor: 'white',
                    bodyColor: 'white',
                    borderColor: 'rgba(255, 255, 255, 0.3)',
                    borderWidth: 1,
                    cornerRadius: 12,
                    padding: 12,
                    displayColors: true,
                    callbacks: {
                        title: function(context) {
                            return context[0].label || '';
                        },
                        label: function(context) {
                            const value = typeof context.parsed === 'number' 
                                ? context.parsed 
                                : context.parsed.y || context.parsed;
                            return `${context.dataset.label}: $${value.toFixed(2)}`;
                        }
                    }
                }
            },
            elements: {
                arc: {
                    borderRadius: 8,
                    borderWidth: 3
                },
                line: {
                    borderWidth: 3,
                    fill: true,
                    tension: 0.4
                },
                point: {
                    radius: 6,
                    hoverRadius: 8,
                    borderWidth: 2
                },
                bar: {
                    borderRadius: 6,
                    borderSkipped: false
                }
            }
        };
    }

    initSpendingChart(canvasId, totalSpent) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return null;

        const carRentals = totalSpent * 0.75;
        const serviceFees = totalSpent * 0.15;
        const insurance = totalSpent * 0.10;

        const data = {
            labels: ['Car Rentals', 'Service Fees', 'Insurance'],
            datasets: [{
                data: [carRentals, serviceFees, insurance],
                backgroundColor: [
                    'rgba(52, 152, 219, 0.8)',
                    'rgba(46, 204, 113, 0.8)', 
                    'rgba(241, 196, 15, 0.8)'
                ],
                borderColor: [
                    'rgba(52, 152, 219, 1)',
                    'rgba(46, 204, 113, 1)',
                    'rgba(241, 196, 15, 1)'
                ],
                borderWidth: 3,
                hoverOffset: 20,
                cutout: '65%'
            }]
        };

        const options = {
            ...this.chartOptions,
            plugins: {
                ...this.chartOptions.plugins,
                tooltip: {
                    ...this.chartOptions.plugins.tooltip,
                    callbacks: {
                        label: function(context) {
                            const percentage = ((context.parsed / totalSpent) * 100).toFixed(1);
                            return `${context.label}: $${context.parsed.toFixed(2)} (${percentage}%)`;
                        }
                    }
                }
            },
            onHover: (event, activeElements, chart) => {
                chart.canvas.style.cursor = activeElements.length > 0 ? 'pointer' : 'default';
            },
            onClick: (event, activeElements, chart) => {
                if (activeElements.length > 0) {
                    const index = activeElements[0].index;
                    const label = chart.data.labels[index];
                    this.showChartDetail(label, chart.data.datasets[0].data[index]);
                }
            }
        };

        this.charts.spending = new Chart(ctx, {
            type: 'doughnut',
            data: data,
            options: options
        });

        return this.charts.spending;
    }

    initBookingTrendChart(canvasId, bookingData) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return null;

        // Sample monthly booking data
        const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun'];
        const bookings = [2, 4, 3, 5, 7, 6];
        const revenue = [450, 890, 670, 1200, 1650, 1400];

        const data = {
            labels: months,
            datasets: [
                {
                    label: 'Bookings',
                    data: bookings,
                    borderColor: 'rgba(52, 152, 219, 1)',
                    backgroundColor: 'rgba(52, 152, 219, 0.1)',
                    yAxisID: 'y',
                    tension: 0.4,
                    pointBackgroundColor: 'rgba(52, 152, 219, 1)',
                    pointBorderColor: '#fff',
                    pointBorderWidth: 2
                },
                {
                    label: 'Revenue ($)',
                    data: revenue,
                    borderColor: 'rgba(46, 204, 113, 1)',
                    backgroundColor: 'rgba(46, 204, 113, 0.1)',
                    yAxisID: 'y1',
                    tension: 0.4,
                    pointBackgroundColor: 'rgba(46, 204, 113, 1)',
                    pointBorderColor: '#fff',
                    pointBorderWidth: 2
                }
            ]
        };

        const options = {
            ...this.chartOptions,
            scales: {
                x: {
                    grid: {
                        display: false
                    },
                    ticks: {
                        font: {
                            size: 11
                        }
                    }
                },
                y: {
                    type: 'linear',
                    display: true,
                    position: 'left',
                    title: {
                        display: true,
                        text: 'Bookings'
                    },
                    grid: {
                        color: 'rgba(0, 0, 0, 0.1)'
                    }
                },
                y1: {
                    type: 'linear',
                    display: true,
                    position: 'right',
                    title: {
                        display: true,
                        text: 'Revenue ($)'
                    },
                    grid: {
                        drawOnChartArea: false
                    }
                }
            },
            plugins: {
                ...this.chartOptions.plugins,
                tooltip: {
                    ...this.chartOptions.plugins.tooltip,
                    callbacks: {
                        label: function(context) {
                            const label = context.dataset.label;
                            const value = context.parsed.y;
                            return label === 'Revenue ($)' 
                                ? `${label}: $${value.toFixed(2)}`
                                : `${label}: ${value}`;
                        }
                    }
                }
            }
        };

        this.charts.bookingTrend = new Chart(ctx, {
            type: 'line',
            data: data,
            options: options
        });

        return this.charts.bookingTrend;
    }

    initCarTypeChart(canvasId) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return null;

        const data = {
            labels: ['Economy', 'Compact', 'Midsize', 'Full-size', 'SUV', 'Luxury'],
            datasets: [{
                label: 'Bookings by Car Type',
                data: [12, 8, 15, 10, 6, 4],
                backgroundColor: [
                    'rgba(52, 152, 219, 0.8)',
                    'rgba(46, 204, 113, 0.8)',
                    'rgba(241, 196, 15, 0.8)',
                    'rgba(230, 126, 34, 0.8)',
                    'rgba(155, 89, 182, 0.8)',
                    'rgba(231, 76, 60, 0.8)'
                ],
                borderColor: [
                    'rgba(52, 152, 219, 1)',
                    'rgba(46, 204, 113, 1)',
                    'rgba(241, 196, 15, 1)',
                    'rgba(230, 126, 34, 1)',
                    'rgba(155, 89, 182, 1)',
                    'rgba(231, 76, 60, 1)'
                ],
                borderWidth: 2,
                hoverBorderWidth: 3
            }]
        };

        const options = {
            ...this.chartOptions,
            scales: {
                y: {
                    beginAtZero: true,
                    grid: {
                        color: 'rgba(0, 0, 0, 0.1)'
                    },
                    ticks: {
                        stepSize: 2
                    }
                },
                x: {
                    grid: {
                        display: false
                    }
                }
            },
            plugins: {
                ...this.chartOptions.plugins,
                tooltip: {
                    ...this.chartOptions.plugins.tooltip,
                    callbacks: {
                        label: function(context) {
                            const percentage = ((context.parsed.y / 55) * 100).toFixed(1);
                            return `${context.label}: ${context.parsed.y} bookings (${percentage}%)`;
                        }
                    }
                }
            }
        };

        this.charts.carType = new Chart(ctx, {
            type: 'bar',
            data: data,
            options: options
        });

        return this.charts.carType;
    }

    initRadarChart(canvasId, userPreferences) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return null;

        const data = {
            labels: ['Price Sensitivity', 'Luxury Preference', 'Eco-Friendly', 'Performance', 'Space Needs', 'Technology'],
            datasets: [{
                label: 'Your Preferences',
                data: [7, 4, 8, 6, 5, 9],
                borderColor: 'rgba(52, 152, 219, 1)',
                backgroundColor: 'rgba(52, 152, 219, 0.2)',
                pointBackgroundColor: 'rgba(52, 152, 219, 1)',
                pointBorderColor: '#fff',
                pointBorderWidth: 2,
                pointRadius: 5
            }, {
                label: 'Average User',
                data: [6, 5, 6, 7, 6, 6],
                borderColor: 'rgba(46, 204, 113, 1)',
                backgroundColor: 'rgba(46, 204, 113, 0.1)',
                pointBackgroundColor: 'rgba(46, 204, 113, 1)',
                pointBorderColor: '#fff',
                pointBorderWidth: 2,
                pointRadius: 5
            }]
        };

        const options = {
            ...this.chartOptions,
            scales: {
                r: {
                    beginAtZero: true,
                    max: 10,
                    ticks: {
                        stepSize: 2,
                        font: {
                            size: 10
                        }
                    },
                    grid: {
                        color: 'rgba(0, 0, 0, 0.1)'
                    }
                }
            }
        };

        this.charts.radar = new Chart(ctx, {
            type: 'radar',
            data: data,
            options: options
        });

        return this.charts.radar;
    }

    showChartDetail(label, value) {
        // Create a modal or notification showing chart details
        const detail = `
            <div class="chart-detail-popup">
                <h6>${label}</h6>
                <p>Amount: $${value.toFixed(2)}</p>
                <small>Click to view detailed breakdown</small>
            </div>
        `;
        
        // Show notification (assuming notification system exists)
        if (window.dashboardInstance) {
            window.dashboardInstance.showNotification(`${label}: $${value.toFixed(2)}`, 'info', 3000);
        }
    }

    updateChartPeriod(chartType, period) {
        // Update chart data based on selected period
        const chart = this.charts[chartType];
        if (!chart) return;

        // Simulate data update based on period
        switch (period) {
            case 'month':
                this.updateMonthlyData(chart);
                break;
            case 'year':
                this.updateYearlyData(chart);
                break;
            case 'all':
                this.updateAllTimeData(chart);
                break;
        }

        chart.update('active');
    }

    updateMonthlyData(chart) {
        // Update with current month data
        if (chart.config.type === 'doughnut') {
            chart.data.datasets[0].data = [450, 85, 65];
            chart.update();
        }
    }

    updateYearlyData(chart) {
        // Update with current year data
        if (chart.config.type === 'doughnut') {
            chart.data.datasets[0].data = [5400, 980, 720];
            chart.update();
        }
    }

    updateAllTimeData(chart) {
        // Update with all-time data
        if (chart.config.type === 'doughnut') {
            chart.data.datasets[0].data = [12500, 2250, 1750];
            chart.update();
        }
    }

    addHoverEffects() {
        // Add custom hover effects to chart containers
        Object.values(this.charts).forEach(chart => {
            const container = chart.canvas.parentElement;
            
            container.addEventListener('mouseenter', () => {
                container.style.transform = 'scale(1.02)';
                container.style.transition = 'transform 0.3s ease';
            });
            
            container.addEventListener('mouseleave', () => {
                container.style.transform = 'scale(1)';
            });
        });
    }

    addChartAnimation() {
        // Add entrance animations to charts
        Object.values(this.charts).forEach((chart, index) => {
            const container = chart.canvas.parentElement;
            container.style.opacity = '0';
            container.style.transform = 'translateY(20px)';
            
            setTimeout(() => {
                container.style.transition = 'all 0.6s ease';
                container.style.opacity = '1';
                container.style.transform = 'translateY(0)';
            }, index * 200);
        });
    }

    destroy() {
        // Clean up all charts
        Object.values(this.charts).forEach(chart => {
            chart.destroy();
        });
        this.charts = {};
    }
}

// Global chart instance
let chartManager = null;

// Initialize charts when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    // Wait for dashboard to initialize first
    setTimeout(() => {
        chartManager = new DashboardCharts();
        
        // Initialize charts if elements exist
        const spendingChart = chartManager.initSpendingChart('spendingChart', window.totalSpent || 1500);
        const trendChart = chartManager.initBookingTrendChart('bookingTrendChart');
        const carTypeChart = chartManager.initCarTypeChart('carTypeChart');
        const radarChart = chartManager.initRadarChart('preferenceRadarChart');
        
        // Add effects
        chartManager.addHoverEffects();
        chartManager.addChartAnimation();
        
        // Set up period change handlers
        const periodSelector = document.getElementById('chart-period');
        if (periodSelector) {
            periodSelector.addEventListener('change', function(e) {
                chartManager.updateChartPeriod('spending', e.target.value);
            });
        }
        
        console.log('📊 Dashboard charts initialized successfully!');
    }, 1000);
});

// Clean up on page unload
window.addEventListener('beforeunload', function() {
    if (chartManager) {
        chartManager.destroy();
    }
});

// Export for global use
window.DashboardCharts = DashboardCharts;