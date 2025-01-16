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
            pageSize: paramArray.pageSize || 100,
            orderBy: 'created desc',
            pageNumber: paramArray.pageNumber || 1
        },
        mounted: function () {
            naite.vuemodel = this;
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
        if (item.pageSize) query += "&pageSize=" + encodeURIComponent(item.pageSize);
        if (item.pageNumber) query += "&pageNumber=" + encodeURIComponent(item.pageNumber);
        if (item.orderBy) query += "&orderBy=" + encodeURIComponent(item.orderBy);
        if (query.length > 0) query = "?" + query.substr(1);
        window.history.replaceState(null, null, $(location).attr('pathname') + query);

        naite.get(naite.apiUrls.itemDataImports, item)
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
                                $('<td></td>').text(i + 1)
                            )
                            .append(
                                $('<td></td>').append(data[i].originalFileName)
                            )
                            .append(
                                $('<td></td>').append(data[i].reserved == null ? null : naite.dateTimeOffsetSecFullToString(data[i].reserved))
                            )
                            .append(
                                $('<td></td>').append(data[i].imported == null ? null : naite.dateTimeOffsetSecFullToString(data[i].imported))
                            )
                            .append(
                                $('<td></td>').append(data[i].userName)
                            )
                            .append(
                                $('<td></td>').append(data[i].number)
                            )
                            .append(
                                $('<td class="text-center" style="width:40px;"></td>')
                                    .append(
                                        $('<a></a>')
                                            .addClass(data[i].imported == null ? 'text-teal' : 'text-secondary')
                                            .attr('href', data[i].imported == null ? naite.urls.itemDataImportEdit(data[i].id) : '#')
                                            .append(
                                                $('<i class="fa fa-edit"  style="font-size:x-large;"></i>')
                                            )
                                    )
                            )
                            .append(
                                $('<td class="text-center" style="width:40px;"></td>')
                                    .append(
                                        $('<a class="text-teal download" href="#"></a>')
                                            .attr('data-id', data[i].id)
                                            .append(
                                                $('<i class="fas fa-download style="font-size:x-large;"></i>')
                                            )
                                    )
                            )
                            .append(
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
            title: "削除しますか？",
            text: "",
            type: "warning",
            showCancelButton: true,
            confirmButtonText: "はい",
            cancelButtonText: "いいえ"
        }).then(function (isConfirm) {
            if (isConfirm.value) {
                naite.delete(naite.apiUrls.itemDataImport(id), null)
                    .done(function (data) {
                        console.log(data);
                        swal({
                            title: "削除が完了しました",
                            text: "",
                            type: "success"
                        }).then(function () {
                            naite.redirect(naite.urls.itemDataImports);
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

    function downloadItem(id) {
        naite.FileDownload(naite.apiUrls.downloadItemDataImport(id), null)
            .done(function (data) {
                console.log(data);
                var str = Encoding.stringToCode(data);
                var convert = Encoding.convert(str, 'utf8', 'unicode');
                var u8a = new Uint8Array(convert);
                let bom = new Uint8Array([0xEF, 0xBB, 0xBF]);
                var blob = new Blob([bom, u8a], { type: 'text/csv' });
                var url = URL.createObjectURL(blob);
                let element = document.createElement('a');
                element.href = url;
                element.download = 'item_import_data.csv';
                element.target = '_blank';
                element.click();
            })
            .fail(function (error) {
                console.log(error);
                swal("ダウンロードに失敗しました", "", "error");
            });
    }

    $(document).on("click", "a.delete", function (event) {
        event.preventDefault();
        deleteItem($(this).data('id'));
    });

    $(document).on("click", "a.download", function (event) {
        event.preventDefault();
        downloadItem($(this).data('id'));
    });
});