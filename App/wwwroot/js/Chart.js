$(document).ready(() => {
    fetch('/Home/GetNoOfTask')
        .then(response => response.json())
        .then(data => {
            showPie(data.data);
        });

    function showPie(dt) {
        var donutData = {
            labels: [
                'Pending',
                'Working',
                'Completed',
                'Canceled',
            ],
            datasets: [
                {
                    label: 'Task',
                    data: [dt.pendingTask, dt.workingTask, dt.completedTask, dt.canceledTask],
                    backgroundColor: ['#dc3545', '#ffc107', '#28a745', '#343a60']
                },
            ]
        };

        //-------------
        //- PIE CHART -
        //-------------
        // Get context with jQuery - using jQuery's .get() method.
        var pieChartCanvas = $('#pieChart').get(0).getContext('2d')
        var pieData = donutData;
        var pieOptions = {
            maintainAspectRatio: false,
            responsive: true,
            hoverOffset: 4,
            plugins: {
                legend: {
                    position: 'top',
                },
                title: {
                    display: true,
                    text: 'Task Pie Chart',
                    padding: {
                        top: 10,
                        bottom: 30
                    }
                }
            }
        }
        //Create pie or douhnut chart
        // You can switch between pie and douhnut using the method below.
        new Chart(pieChartCanvas, {
            type: 'pie',
            data: pieData,
            options: pieOptions,
        })
    }

});