$(function () {
    var myChart = null;
    // パラメータをチェックしてvalueにセット
    var paramArray = naite.getParam() || {};
    naite.vuemodel = new Vue({
        el: '#my-app',
        data: {
            code: naite.getPathId(),
            startDate: paramArray.startDate || $.cookie('startDate') || null,
            endDate: paramArray.endDate || $.cookie('endDate') || null,
            labelType: paramArray.labelType || $.cookie('labelType') || 'day',
            term: paramArray.term || $.cookie('term') || null,
            terms: [
                { text: 'カスタム期間', value: null, plus: 0, minus: 0 },
                { text: '+3月', value: 1, plus: 3, minus: 0 },
                { text: '+6月', value: 2, plus: 6, minus: 0 },
                { text: '+12月', value: 3, plus: 12, minus: 0 },
                { text: '-1月+3月', value: 4, plus: 3, minus: 1 },
                { text: '-1月+6月', value: 5, plus: 6, minus: 1 },
                { text: '-1月+12月', value: 6, plus: 12, minus: 1 },
                { text: '-3月+3月', value: 7, plus: 3, minus: 3 },
                { text: '-3月+6月', value: 8, plus: 6, minus: 3 },
                { text: '-3月+12月', value: 9, plus: 12, minus: 3 },
                { text: '-6月+3月', value: 10, plus: 3, minus: 6 },
                { text: '-6月+6月', value: 11, plus: 6, minus: 6 },
                { text: '-6月+12月', value: 12, plus: 12, minus: 6 },
                { text: '-12月+3月', value: 13, plus: 3, minus: 12 },
                { text: '-12月+6月', value: 14, plus: 6, minus: 12 },
                { text: '-12月+12月', value: 15, plus: 12, minus: 12 }
            ]
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
                            if (item.startDate) {
                                query += "&startDate=" + encodeURIComponent(item.startDate);
                                $.cookie('startDate', item.startDate, { expires: 30, path: "/" });
                            } else {
                                $.removeCookie('startDate', { path: '/' });
                            }
                            if (item.endDate) {
                                query += "&endDate=" + encodeURIComponent(item.endDate);
                                $.cookie('endDate', item.endDate, { expires: 30, path: "/" });
                            } else {
                                $.removeCookie('endDate', { path: '/' });
                            }
                            if (item.labelType) {
                                query += "&labelType=" + encodeURIComponent(item.labelType);
                                $.cookie('labelType', item.labelType, { expires: 30, path: "/" });
                            } else {
                                $.removeCookie('labelType', { path: '/' });
                            }
                            if (item.term) {
                                query += "&term=" + encodeURIComponent(item.term);
                                $.cookie('term', item.term, { expires: 30, path: "/" });
                            } else {
                                $.removeCookie('term', { path: '/' });
                            } 
                            if (query.length > 0) query = "?" + query.substr(1);
                            window.history.replaceState(null, null, $(location).attr('pathname') + query);

                            param = {
                                Code: this.code,
                                StartDate: this.startDate,
                                EndDate: this.endDate,
                                LabelType: this.labelType
                            };

                            if (this.term != null) {
                                let t = this.terms.filter(a => a.value == this.term)[0];
                                let today = new Date();
                                let _nextMonth = new Date(today);
                                _nextMonth.setMonth(_nextMonth.getMonth() + t.plus);
                                console.log(_nextMonth);
                                nextMonth = new Date(_nextMonth.getFullYear(), _nextMonth.getMonth() + 1, 0);
                                console.log(nextMonth);
                                let prevMonth = new Date(today);
                                prevMonth.setMonth(prevMonth.getMonth() - t.minus);
                                console.log(prevMonth);
                                param.StartDate = naite.dateFormatToString(prevMonth, 'YYYY-MM-DD');
                                param.EndDate = naite.dateFormatToString(nextMonth, 'YYYY-MM-DD');
                            }

                            console.log(param);

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