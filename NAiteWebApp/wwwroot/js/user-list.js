$(function () {

    // メッセージ表示
    naite.checkParam();
    // ページ
    naite.initPagination(getPage);
    // パラメータをチェックしてvalueにセット
    var paramArray = naite.getParam() || {};

    naite.vuemodel = new Vue({
        el: '#my-app',
        data: {
            keyword: paramArray.keyword || null,
            isAdmin: false,
            pageSize: paramArray.pageSize || 100,
            orderBy: 'loginId asc',
            pageNumber: paramArray.pageNumber || 1
        },
        mounted: function () {
            naite.vuemodel = this;
            var authority = $.cookie('authority');
            if (authority == 'admin') {
                this.isAdmin = true;
            } else {
                this.isAdmin = false;
            }
            getPage(paramArray.pageNumber || 1);
        },
        methods: {
            updateFilter: function () {
                getPage(1);
            },
            doSubmit: async function (event) {
                this.$validator.validateAll()
                    .then((result) => {
                        if (result) {
                            getPage(1);
                        }
                    });
            }
        }
    });

    function getPage(pageNumber) {
        // 二重クリック防止
        naite.loading(true);

        // パラメータ
        var item = JSON.parse(JSON.stringify(naite.vuemodel.$data));
        item.pageNumber = pageNumber;

        // URLに新しいクエリストリングを付与
        var query = "";
        if (item.keyword) query += "&keyword=" + encodeURIComponent(item.keyword);
        if (item.pageSize) query += "&pageSize=" + encodeURIComponent(item.pageSize);
        if (item.pageNumber) query += "&pageNumber=" + encodeURIComponent(item.pageNumber);
        if (item.orderBy) query += "&orderBy=" + encodeURIComponent(item.orderBy);
        if (query.length > 0) query = "?" + query.substr(1);
        window.history.replaceState(null, null, $(location).attr('pathname') + query);

        naite.get(naite.apiUrls.users, item)
            .done(function (data, status, xhr) {
                console.log(data);
                // ページング作成
                var _pagination = xhr.getResponseHeader('x-pagination');
                var pagination = JSON.parse(_pagination);
                naite.createPagination(pagination.TotalPages, pagination.CurrentPage, 10);

                $('.dataTable tbody').empty();
                for (var i = 0; i < data.length; i++) {
                    $('.dataTable tbody').append(
                        $('<tr></tr>')
                            .append(
                                $('<td></td>').text(data[i].loginId)
                            )
                            .append(
                                $('<td></td>').text(data[i].lastName + ' ' + data[i].firstName)
                            )
                            .append(
                                $('<td></td>').text(data[i].authority == 'admin' ? '管理者' : 'ユーザ')
                        )
                            .append(naite.vuemodel.$data.isAdmin == false ? '' :  
                                $('<td class="text-center" style="width:40px;"></td>')
                                    .append(
                                        $('<a class="text-teal"></a>')
                                            .attr('href', naite.urls.userEdit(data[i].id))
                                            .append(
                                                $('<i class="fa fa-edit"  style="font-size:x-large;"></i>')
                                            )
                                    )
                            )
                            .append(naite.vuemodel.$data.isAdmin == false ? '' :  
                                $('<td class="text-center" style="width:40px;"></td>')
                                    .append(
                                        $('<a class="text-danger delete" href="#"></a>')
                                            .attr('data-id', data[i].id)
                                            .append(
                                                $('<i class="fa fa-trash" style="font-size:x-large;"></i>')
                                            )
                                    )
                            )
                    );
                }

                naite.loading(false);
            })
            .fail(function (error) {
                console.log(error);
                // naite.handleErrorで、naite.loading(false);
                // をしている
                naite.handleError(error);
            });
    }

    function deleteItem(id) {
        swal({
            title: "ユーザを削除しますか？",
            text: "",
            type: "warning",
            showCancelButton: true,
            confirmButtonText: "はい",
            cancelButtonText: "いいえ"
        }).then(function (isConfirm) {
            if (isConfirm.value) {
                naite.delete(naite.apiUrls.user(id), null)
                    .done(function (data) {
                        console.log(data);
                        swal({
                            title: "削除が完了しました",
                            text: "",
                            type: "success"
                        }).then(function () {
                            naite.redirect(naite.urls.users);
                        });
                    })
                    .fail(function (error) {
                        console.log(error);
                        swal("削除に失敗しました", "", "error");
                    });
            } else {
                swal("キャンセルしました", "", "error");
            }
        });
    }

    $(document).on("click", "a.delete", function (event) {
        event.preventDefault();
        deleteItem($(this).data('id'));
    });
});