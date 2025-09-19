// Advanced Dashboard Widgets

class DashboardWidgets {
    constructor() {
        this.widgets = {};
        this.updateIntervals = {};
        this.isInitialized = false;
    }

    init() {
        if (this.isInitialized) return;
        
        console.log('🎯 Initializing Dashboard Widgets...');
        
        this.initNotificationCenter();
        this.initActivityFeed();
        this.initQuickStats();
        this.initProgressTrackers();
        this.initMiniCalendar();
        this.initWeatherWidget();
        this.initPerformanceMetrics();
        
        this.isInitialized = true;
        console.log('✅ Dashboard Widgets initialized successfully!');
    }

    initNotificationCenter() {
        const container = document.getElementById('notification-center');
        if (!container) return;

        const notificationCenter = {
            notifications: [
                {
                    id: 1,
                    type: 'booking',
                    title: 'Booking Reminder',
                    message: 'Your Toyota Camry pickup is in 2 hours',
                    time: '2 hours',
                    icon: 'bi-calendar-event',
                    priority: 'high',
                    read: false
                },
                {
                    id: 2,
                    type: 'payment',
                    title: 'Payment Successful',
                    message: 'Payment of $125.00 has been processed',
                    time: '1 day',
                    icon: 'bi-credit-card',
                    priority: 'medium',
                    read: false
                },
                {
                    id: 3,
                    type: 'system',
                    title: 'Profile Updated',
                    message: 'Your profile information was updated successfully',
                    time: '3 days',
                    icon: 'bi-person-check',
                    priority: 'low',
                    read: true
                }
            ],
            
            render() {
                const unreadCount = this.notifications.filter(n => !n.read).length;
                
                container.innerHTML = `
                    <div class="notification-center-widget">
                        <div class="widget-header">
                            <h6><i class="bi bi-bell"></i> Notifications</h6>
                            <span class="notification-badge">${unreadCount}</span>
                        </div>
                        <div class="notification-list">
                            ${this.notifications.map(notification => this.renderNotification(notification)).join('')}
                        </div>
                        <div class="widget-footer">
                            <button class="btn btn-sm btn-outline-primary w-100" onclick="dashboardWidgets.viewAllNotifications()">
                                View All Notifications
                            </button>
                        </div>
                    </div>
                `;
                
                this.attachEventListeners();
            },
            
            renderNotification(notification) {
                const priorityClass = `priority-${notification.priority}`;
                const readClass = notification.read ? 'read' : 'unread';
                
                return `
                    <div class="notification-item ${readClass} ${priorityClass}" data-id="${notification.id}">
                        <div class="notification-icon">
                            <i class="${notification.icon}"></i>
                        </div>
                        <div class="notification-content">
                            <h6>${notification.title}</h6>
                            <p>${notification.message}</p>
                            <small>${notification.time} ago</small>
                        </div>
                        <div class="notification-actions">
                            ${!notification.read ? '<button class="btn btn-sm mark-read" onclick="dashboardWidgets.markAsRead(' + notification.id + ')"><i class="bi bi-check"></i></button>' : ''}
                            <button class="btn btn-sm delete-notification" onclick="dashboardWidgets.deleteNotification(' + notification.id + ')"><i class="bi bi-x"></i></button>
                        </div>
                    </div>
                `;
            },
            
            attachEventListeners() {
                container.querySelectorAll('.notification-item').forEach(item => {
                    item.addEventListener('click', (e) => {
                        if (!e.target.closest('.notification-actions')) {
                            const id = parseInt(item.dataset.id);
                            this.markAsRead(id);
                        }
                    });
                });
            },
            
            markAsRead(id) {
                const notification = this.notifications.find(n => n.id === id);
                if (notification) {
                    notification.read = true;
                    this.render();
                }
            },
            
            deleteNotification(id) {
                this.notifications = this.notifications.filter(n => n.id !== id);
                this.render();
            },
            
            addNotification(notification) {
                notification.id = Date.now();
                notification.time = 'Just now';
                notification.read = false;
                this.notifications.unshift(notification);
                this.render();
            }
        };

        this.widgets.notificationCenter = notificationCenter;
        notificationCenter.render();
    }

    initActivityFeed() {
        const container = document.getElementById('activity-feed');
        if (!container) return;

        const activityFeed = {
            activities: [
                {
                    type: 'booking',
                    action: 'created',
                    description: 'New booking for Honda Civic',
                    time: new Date(Date.now() - 2 * 60 * 60 * 1000),
                    icon: 'bi-plus-circle',
                    color: 'success'
                },
                {
                    type: 'payment',
                    action: 'completed',
                    description: 'Payment of $89.99 processed',
                    time: new Date(Date.now() - 6 * 60 * 60 * 1000),
                    icon: 'bi-credit-card',
                    color: 'primary'
                },
                {
                    type: 'profile',
                    action: 'updated',
                    description: 'Profile picture updated',
                    time: new Date(Date.now() - 24 * 60 * 60 * 1000),
                    icon: 'bi-person',
                    color: 'info'
                },
                {
                    type: 'booking',
                    action: 'completed',
                    description: 'Toyota Prius rental completed',
                    time: new Date(Date.now() - 3 * 24 * 60 * 60 * 1000),
                    icon: 'bi-check-circle',
                    color: 'success'
                }
            ],
            
            render() {
                container.innerHTML = `
                    <div class="activity-feed-widget">
                        <div class="widget-header">
                            <h6><i class="bi bi-activity"></i> Recent Activity</h6>
                            <div class="activity-filter">
                                <select class="form-select form-select-sm" onchange="dashboardWidgets.filterActivities(this.value)">
                                    <option value="all">All Activities</option>
                                    <option value="booking">Bookings</option>
                                    <option value="payment">Payments</option>
                                    <option value="profile">Profile</option>
                                </select>
                            </div>
                        </div>
                        <div class="activity-timeline">
                            ${this.activities.map(activity => this.renderActivity(activity)).join('')}
                        </div>
                    </div>
                `;
            },
            
            renderActivity(activity) {
                const timeAgo = this.getTimeAgo(activity.time);
                
                return `
                    <div class="activity-item" data-type="${activity.type}">
                        <div class="activity-marker bg-${activity.color}">
                            <i class="${activity.icon}"></i>
                        </div>
                        <div class="activity-content">
                            <div class="activity-description">
                                ${activity.description}
                            </div>
                            <div class="activity-time">
                                ${timeAgo}
                            </div>
                        </div>
                    </div>
                `;
            },
            
            getTimeAgo(date) {
                const now = new Date();
                const diff = now - date;
                const minutes = Math.floor(diff / 60000);
                const hours = Math.floor(minutes / 60);
                const days = Math.floor(hours / 24);
                
                if (days > 0) return `${days} day${days > 1 ? 's' : ''} ago`;
                if (hours > 0) return `${hours} hour${hours > 1 ? 's' : ''} ago`;
                if (minutes > 0) return `${minutes} minute${minutes > 1 ? 's' : ''} ago`;
                return 'Just now';
            },
            
            filterActivities(type) {
                const items = container.querySelectorAll('.activity-item');
                items.forEach(item => {
                    if (type === 'all' || item.dataset.type === type) {
                        item.style.display = 'flex';
                    } else {
                        item.style.display = 'none';
                    }
                });
            },
            
            addActivity(activity) {
                activity.time = new Date();
                this.activities.unshift(activity);
                if (this.activities.length > 10) {
                    this.activities = this.activities.slice(0, 10);
                }
                this.render();
            }
        };

        this.widgets.activityFeed = activityFeed;
        activityFeed.render();
    }

    initQuickStats() {
        const container = document.getElementById('quick-stats');
        if (!container) return;

        const quickStats = {
            stats: [
                { label: 'This Month', value: '$450', change: '+12%', icon: 'bi-graph-up', trend: 'up' },
                { label: 'Avg Rating', value: '4.8', change: '+0.2', icon: 'bi-star-fill', trend: 'up' },
                { label: 'Total Miles', value: '2,450', change: '+156', icon: 'bi-speedometer2', trend: 'up' },
                { label: 'Carbon Saved', value: '45kg', change: '+8kg', icon: 'bi-tree-fill', trend: 'up' }
            ],
            
            render() {
                container.innerHTML = `
                    <div class="quick-stats-widget">
                        <div class="stats-grid">
                            ${this.stats.map(stat => this.renderStat(stat)).join('')}
                        </div>
                    </div>
                `;
            },
            
            renderStat(stat) {
                return `
                    <div class="stat-item">
                        <div class="stat-icon">
                            <i class="${stat.icon}"></i>
                        </div>
                        <div class="stat-content">
                            <div class="stat-value">${stat.value}</div>
                            <div class="stat-label">${stat.label}</div>
                            <div class="stat-change trend-${stat.trend}">
                                <i class="bi bi-arrow-${stat.trend}"></i>
                                ${stat.change}
                            </div>
                        </div>
                    </div>
                `;
            }
        };

        this.widgets.quickStats = quickStats;
        quickStats.render();
    }

    initProgressTrackers() {
        const container = document.getElementById('progress-trackers');
        if (!container) return;

        const progressTrackers = {
            trackers: [
                { label: 'Profile Completion', value: 85, max: 100, color: 'success' },
                { label: 'Monthly Goal', value: 3, max: 5, color: 'primary' },
                { label: 'Loyalty Points', value: 750, max: 1000, color: 'warning' },
                { label: 'Eco Score', value: 70, max: 100, color: 'info' }
            ],
            
            render() {
                container.innerHTML = `
                    <div class="progress-trackers-widget">
                        <div class="widget-header">
                            <h6><i class="bi bi-speedometer2"></i> Progress Trackers</h6>
                        </div>
                        <div class="progress-list">
                            ${this.trackers.map(tracker => this.renderTracker(tracker)).join('')}
                        </div>
                    </div>
                `;
                
                // Animate progress bars
                setTimeout(() => {
                    container.querySelectorAll('.progress-bar').forEach(bar => {
                        const width = bar.style.width;
                        bar.style.width = '0%';
                        bar.style.transition = 'width 1s ease-out';
                        setTimeout(() => {
                            bar.style.width = width;
                        }, 100);
                    });
                }, 500);
            },
            
            renderTracker(tracker) {
                const percentage = (tracker.value / tracker.max) * 100;
                
                return `
                    <div class="progress-tracker-item">
                        <div class="tracker-header">
                            <span class="tracker-label">${tracker.label}</span>
                            <span class="tracker-value">${tracker.value}/${tracker.max}</span>
                        </div>
                        <div class="progress" style="height: 8px;">
                            <div class="progress-bar bg-${tracker.color}" 
                                 style="width: ${percentage}%"></div>
                        </div>
                        <div class="tracker-percentage">${Math.round(percentage)}%</div>
                    </div>
                `;
            }
        };

        this.widgets.progressTrackers = progressTrackers;
        progressTrackers.render();
    }

    initMiniCalendar() {
        const container = document.getElementById('mini-calendar');
        if (!container) return;

        const miniCalendar = {
            currentDate: new Date(),
            events: [
                { date: new Date(), type: 'pickup', title: 'Car Pickup - Honda Civic' },
                { date: new Date(Date.now() + 2 * 24 * 60 * 60 * 1000), type: 'return', title: 'Car Return - Honda Civic' },
                { date: new Date(Date.now() + 5 * 24 * 60 * 60 * 1000), type: 'booking', title: 'Toyota Camry Booking' }
            ],
            
            render() {
                const today = new Date();
                const monthNames = ['January', 'February', 'March', 'April', 'May', 'June',
                    'July', 'August', 'September', 'October', 'November', 'December'];
                
                container.innerHTML = `
                    <div class="mini-calendar-widget">
                        <div class="calendar-header">
                            <h6>${monthNames[today.getMonth()]} ${today.getFullYear()}</h6>
                            <button class="btn btn-sm btn-outline-primary" onclick="dashboardWidgets.showFullCalendar()">
                                <i class="bi bi-calendar3"></i>
                            </button>
                        </div>
                        <div class="calendar-grid">
                            ${this.renderCalendarDays()}
                        </div>
                        <div class="upcoming-events">
                            <h6>Upcoming</h6>
                            ${this.renderUpcomingEvents()}
                        </div>
                    </div>
                `;
            },
            
            renderCalendarDays() {
                const today = new Date();
                const firstDay = new Date(today.getFullYear(), today.getMonth(), 1);
                const lastDay = new Date(today.getFullYear(), today.getMonth() + 1, 0);
                const startDate = new Date(firstDay);
                startDate.setDate(startDate.getDate() - firstDay.getDay());
                
                let daysHTML = '<div class="calendar-days-header">';
                ['S', 'M', 'T', 'W', 'T', 'F', 'S'].forEach(day => {
                    daysHTML += `<div class="day-header">${day}</div>`;
                });
                daysHTML += '</div><div class="calendar-days">';
                
                for (let i = 0; i < 42; i++) {
                    const currentDate = new Date(startDate);
                    currentDate.setDate(startDate.getDate() + i);
                    
                    const isToday = currentDate.toDateString() === today.toDateString();
                    const isCurrentMonth = currentDate.getMonth() === today.getMonth();
                    const hasEvent = this.events.some(event => 
                        event.date.toDateString() === currentDate.toDateString()
                    );
                    
                    let dayClass = 'calendar-day';
                    if (isToday) dayClass += ' today';
                    if (!isCurrentMonth) dayClass += ' other-month';
                    if (hasEvent) dayClass += ' has-event';
                    
                    daysHTML += `<div class="${dayClass}">${currentDate.getDate()}</div>`;
                }
                
                daysHTML += '</div>';
                return daysHTML;
            },
            
            renderUpcomingEvents() {
                const upcomingEvents = this.events
                    .filter(event => event.date >= new Date())
                    .sort((a, b) => a.date - b.date)
                    .slice(0, 3);
                
                if (upcomingEvents.length === 0) {
                    return '<p class="text-muted">No upcoming events</p>';
                }
                
                return upcomingEvents.map(event => `
                    <div class="event-item">
                        <div class="event-date">${event.date.getDate()}/${event.date.getMonth() + 1}</div>
                        <div class="event-details">
                            <div class="event-title">${event.title}</div>
                            <div class="event-type badge bg-${this.getEventColor(event.type)}">${event.type}</div>
                        </div>
                    </div>
                `).join('');
            },
            
            getEventColor(type) {
                const colors = {
                    pickup: 'success',
                    return: 'warning',
                    booking: 'primary',
                    maintenance: 'info'
                };
                return colors[type] || 'secondary';
            }
        };

        this.widgets.miniCalendar = miniCalendar;
        miniCalendar.render();
    }

    initWeatherWidget() {
        // Already implemented in main dashboard script, just enhance it
        const weatherWidget = document.querySelector('.weather-widget');
        if (weatherWidget) {
            // Add click handler for detailed weather
            weatherWidget.addEventListener('click', () => {
                this.showDetailedWeather();
            });
            
            weatherWidget.style.cursor = 'pointer';
            weatherWidget.title = 'Click for detailed weather forecast';
        }
    }

    initPerformanceMetrics() {
        const container = document.getElementById('performance-metrics');
        if (!container) return;

        const performanceMetrics = {
            metrics: {
                responseTime: { value: 1.2, unit: 'ms', status: 'good' },
                uptime: { value: 99.9, unit: '%', status: 'excellent' },
                satisfaction: { value: 4.8, unit: '/5', status: 'good' },
                efficiency: { value: 85, unit: '%', status: 'good' }
            },
            
            render() {
                container.innerHTML = `
                    <div class="performance-metrics-widget">
                        <div class="widget-header">
                            <h6><i class="bi bi-speedometer"></i> Performance</h6>
                        </div>
                        <div class="metrics-grid">
                            ${Object.entries(this.metrics).map(([key, metric]) => 
                                this.renderMetric(key, metric)
                            ).join('')}
                        </div>
                    </div>
                `;
            },
            
            renderMetric(key, metric) {
                const statusColors = {
                    excellent: 'success',
                    good: 'primary',
                    warning: 'warning',
                    poor: 'danger'
                };
                
                return `
                    <div class="metric-item">
                        <div class="metric-value text-${statusColors[metric.status]}">
                            ${metric.value}${metric.unit}
                        </div>
                        <div class="metric-label">${key.charAt(0).toUpperCase() + key.slice(1)}</div>
                        <div class="metric-status">
                            <span class="badge bg-${statusColors[metric.status]}">${metric.status}</span>
                        </div>
                    </div>
                `;
            }
        };

        this.widgets.performanceMetrics = performanceMetrics;
        performanceMetrics.render();
    }

    // Widget interaction methods
    markAsRead(notificationId) {
        if (this.widgets.notificationCenter) {
            this.widgets.notificationCenter.markAsRead(notificationId);
        }
    }

    deleteNotification(notificationId) {
        if (this.widgets.notificationCenter) {
            this.widgets.notificationCenter.deleteNotification(notificationId);
        }
    }

    filterActivities(type) {
        if (this.widgets.activityFeed) {
            this.widgets.activityFeed.filterActivities(type);
        }
    }

    viewAllNotifications() {
        // Navigate to notifications page or show modal
        console.log('Opening all notifications view...');
        if (window.dashboardInstance) {
            window.dashboardInstance.showNotification('Opening notifications center...', 'info', 2000);
        }
    }

    showFullCalendar() {
        // Show full calendar modal or navigate to calendar page
        console.log('Opening full calendar view...');
        if (window.dashboardInstance) {
            window.dashboardInstance.showNotification('Opening calendar view...', 'info', 2000);
        }
    }

    showDetailedWeather() {
        // Show detailed weather modal
        const modal = document.createElement('div');
        modal.className = 'modal fade';
        modal.innerHTML = `
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">Weather Forecast</h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                    </div>
                    <div class="modal-body">
                        <div class="weather-forecast">
                            <div class="today-weather">
                                <h6>Today</h6>
                                <div class="weather-details">
                                    <span class="temp">22°C</span>
                                    <span class="condition">Sunny</span>
                                    <span class="humidity">Humidity: 65%</span>
                                    <span class="wind">Wind: 15 km/h</span>
                                </div>
                            </div>
                            <div class="forecast-days">
                                <h6>5-Day Forecast</h6>
                                ${this.generateForecast()}
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        `;
        
        document.body.appendChild(modal);
        const bootstrapModal = new bootstrap.Modal(modal);
        bootstrapModal.show();
        
        modal.addEventListener('hidden.bs.modal', () => {
            modal.remove();
        });
    }

    generateForecast() {
        const days = ['Tomorrow', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
        const temps = [24, 20, 18, 25, 27];
        const conditions = ['Cloudy', 'Rainy', 'Sunny', 'Sunny', 'Partly Cloudy'];
        
        return days.map((day, index) => `
            <div class="forecast-day">
                <span class="day">${day}</span>
                <span class="temp">${temps[index]}°C</span>
                <span class="condition">${conditions[index]}</span>
            </div>
        `).join('');
    }

    refreshAllWidgets() {
        console.log('Refreshing all widgets...');
        Object.values(this.widgets).forEach(widget => {
            if (widget.render) {
                widget.render();
            }
        });
        
        if (window.dashboardInstance) {
            window.dashboardInstance.showNotification('All widgets refreshed!', 'success', 3000);
        }
    }

    destroy() {
        // Clear all update intervals
        Object.values(this.updateIntervals).forEach(interval => {
            clearInterval(interval);
        });
        
        this.widgets = {};
        this.updateIntervals = {};
        this.isInitialized = false;
        
        console.log('🧹 Dashboard Widgets destroyed');
    }
}

// Global instance
let dashboardWidgets = null;

// Initialize widgets after main dashboard
document.addEventListener('DOMContentLoaded', function() {
    setTimeout(() => {
        dashboardWidgets = new DashboardWidgets();
        dashboardWidgets.init();
    }, 1500);
});

// Export for global use
window.DashboardWidgets = DashboardWidgets;
window.dashboardWidgets = dashboardWidgets;