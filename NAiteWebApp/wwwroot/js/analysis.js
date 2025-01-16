$(function () {
    var myChart = null;
    // パラメータをチェックしてvalueにセット
    var paramArray = naite.getParam() || {};
    naite.vuemodel = new Vue({
        el: '#my-app',
        data: {
            code: naite.getPathId(),
            startDate: paramArray.startDate || null,
            endDate: paramArray.endDate || null
        },
        mounted: function () {
            naite.vuemodel = this;
            if (this.startDate == null) {
                var startDate = new Date();
                startDate.setDate(startDate.getDate() - 7);
                this.startDate = naite.dateFormatToString(startDate, 'YYYY-MM-DD');
            }
            if (this.endDate == null) {
                var endDate = new Date();
                endDate.setDate(endDate.getDate() + 7);
                this.endDate = naite.dateFormatToString(endDate, 'YYYY-MM-DD');
            }
            this.getGraph();
        },
        methods: {
            getGraph: async function (event) {
                this.$validator.validateAll()
                    .then((result) => {
                        if (result) {

                            // パラメータ
                            var item = JSON.parse(JSON.stringify(naite.vuemodel.$data));
                            // URLに新しいクエリストリングを付与
                            var query = "";
                            if (item.startDate) query += "&startDate=" + encodeURIComponent(item.startDate);
                            if (item.endDate) query += "&endDate=" + encodeURIComponent(item.endDate);
                            if (query.length > 0) query = "?" + query.substr(1);
                            window.history.replaceState(null, null, $(location).attr('pathname') + query);

                            param = {
                                Code: this.code,
                                StartDate: this.startDate,
                                EndDate: this.endDate
                            };
                            naite.get(naite.apiUrls.itemDatas, param)
                                .done(function (data) {
                                    console.log(data);
                                    // グラフ表示
                                    if (myChart != null) {
                                        myChart.destroy();
                                    }
                                    var xAxisLabelMinWidth = 25; // データ当たりの幅を設定
                                    console.log(data.graphLabels);
                                    var width = data.graphLabels.length * xAxisLabelMinWidth;
                                    if ($('.chartWrapper').width() > width) {
                                        $(".chartContainer").width("100%");
                                        $('.chartWrapper').css('overflow-x', 'unset');
                                        $('.chartWrapper').addClass('mb-3');
                                    } else {
                                        $('.chartWrapper').css('overflow-x', 'scroll');
                                        $('.chartWrapper').removeClass('mb-3');
                                        $(".chartContainer").width(width + "px");
                                    }
                                    var ctx = document.getElementById("myChart").getContext('2d');
                                    let config = {
                                        type: 'line',
                                        data: {
                                            labels: data.graphLabels,
                                            datasets: [{
                                                label: '在庫数',
                                                spanGaps: true,
                                                data: data.graphDatas[0].data,
                                                borderColor: '#02A5F0',
                                            }],
                                        },
                                        options: {
                                            responsive: true,
                                            maintainAspectRatio: false,
                                            plugins: {
                                                legend: {
                                                    display: false,
                                                },
                                                annotation: {
                                                    annotations: {
                                                    }
                                                }
                                            },
                                            scales: {
                                                x: {
                                                },
                                                y: {
                                                }
                                            }
                                        }
                                    };

                                    //  min,maxの設定
                                    if (data.maxValue != null) {
                                        config.options.scales.y.max = data.maxValue;
                                    }
                                    if (data.minValue != null) {
                                        config.options.scales.y.min = data.minValue;
                                    }

                                    // 在庫閾値
                                    if (data.stockThreshold != null) {
                                        const annotation1 = {
                                            type: 'line',
                                            borderColor: '#f44336',
                                            borderWidth: 3,
                                            borderDash: [6, 6],
                                            borderDashOffset: 0,
                                            label: {
                                                display: true,
                                                backgroundColor: '#f44336',
                                                content: '在庫閾値',
                                                position: 'start'
                                            },
                                            scaleID: 'y',
                                            value: data.stockThreshold
                                        };
                                        config.options.plugins.annotation.annotations.annotation1 = annotation1;
                                    }

                                    // 今日
                                    if (data.today != null) {
                                        const annotation2 = {
                                            type: 'line',
                                            borderColor: '#5d5d5d',
                                            borderWidth: 3,
                                            borderDash: [6, 6],
                                            borderDashOffset: 0,
                                            label: {
                                                display: true,
                                                backgroundColor: '#5d5d5d',
                                                content: '今日',
                                                position: 'start'
                                            },
                                            scaleID: 'x',
                                            value: data.today
                                        };
                                        config.options.plugins.annotation.annotations.annotation2 = annotation2;
                                    }

                                    // 棚卸日
                                    if (data.inventoryDate != null) {
                                        const annotation3 = {
                                            type: 'line',
                                            borderColor: '#5d5d5d',
                                            borderWidth: 3,
                                            borderDash: [6, 6],
                                            borderDashOffset: 0,
                                            label: {
                                                display: true,
                                                backgroundColor: '#5d5d5d',
                                                content: '棚卸日',
                                                position: 'start'
                                            },
                                            scaleID: 'x',
                                            value: data.inventoryDate
                                        };
                                        config.options.plugins.annotation.annotations.annotation3 = annotation3;
                                    }
                                    console.log(config);
                                    myChart = new Chart(ctx, config);
                                })
                                .fail(function (error) {
                                    console.log(error);
                                    var msg = naite.handleError(error);
                                    if (msg) {
                                        swal(msg, "", "error");
                                    }
                                });
                        }
                    });
            },
            dateTimeOffsetDayToString: function (date) {
                return naite.dateTimeOffsetDayToString(date);
            },
            dateTimeOffsetTimeToString: function (date) {
                return naite.dateTimeOffsetTimeToString(date);
            },
            dateTimeOffsetToString: function (date) {
                return naite.dateTimeOffsetToString(date, '/');
            },
            changeDate: function () {
                console.log('changeDate');
                this.getGraph();
            },
        }
    });
});