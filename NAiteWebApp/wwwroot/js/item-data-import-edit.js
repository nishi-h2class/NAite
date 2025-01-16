$(function () {

    // メッセージ表示
    naite.checkParam();
    // パラメータをチェックしてvalueにセット
    var paramArray = naite.getParam() || {};

    naite.vuemodel = new Vue({
        el: '#my-app',
        data: {
            id: naite.getPathId(),
            isConfirm: false,
            fields: [],
            rows: [],
            dispRows: [],
            tags: [
                { id: 'code', name: '商品コード' },
                { id: 'date', name: '日付' },
                { id: 'quantity', name: '数量' }
            ],
            fileType: null
        },
        mounted: function () {
            naite.vuemodel = this;
            this.getDataImport();
            this.getDataImportFields();
        },
        methods: {
            getDataImport: function () {
                naite.get(naite.apiUrls.itemDataImport(this.id), null)
                    .done(function (data) {
                        naite.vuemodel.$data.fileType = data.fileType;
                        naite.vuemodel.createDisplayData(data.rows);
                    })
                    .fail(function (error) {
                        console.log(error);
                        // naite.handleErrorで、naite.loading(false);
                        // をしている
                        naite.handleError(error);
                    });
            },
            getDataImportFields: function () {
                var param = {
                    ItemDataImportId: naite.vuemodel.$data.id,
                };
                naite.get(naite.apiUrls.itemDataImportFields, param)
                    .done(function (data) {
                        naite.vuemodel.fields = data;
                    })
                    .fail(function (error) {
                        console.log(error);
                        // naite.handleErrorで、naite.loading(false);
                        // をしている
                        naite.handleError(error);
                    });
            },
            createDisplayData: function (rows) {
                console.log(rows);
                this.rows = rows;

                // 行列入替
                var _dispRows = rows.slice();
                /* ① 行・列を入れ替える転置関数 */
                const transpose = a => a[0].map((_, c) => a.map(r => r[c]));
                this.dispRows = transpose(_dispRows);
            },
            putData: function () {
                // パラメータ
                var item = {
                    Id: naite.vuemodel.$data.id,
                    Fields: naite.vuemodel.$data.fields
                }
                console.log(item);

                naite.put(naite.apiUrls.updateItemDataImportFields(item.Id), item)
                    .done(function (data) {
                        console.log(data);
                        // 二重クリック解除
                        naite.loading(false);
                        swal({
                            title: "登録が完了しました",
                            text: "",
                            type: "success"
                        }).then(function () {
                            naite.redirect(naite.urls.itemDataImports);
                        });
                    })
                    .fail(function (error) {
                        console.log(error);
                        var msg = naite.handleError(error);
                        if (msg) {
                            swal(msg, "", "error");
                        }
                    });
            },
            doSubmit: async function (event) {
                this.$validator.validateAll()
                    .then((result) => {
                        if (result) {

                            if (naite.vuemodel.$data.isConfirm == false) {
                                swal("確認にチェックを入れてください", "", "error");
                                return false;
                            }

                            // タグが選択されているか、2つ以上選択されていないか
                            var codes = naite.vuemodel.$data.fields.filter(a => a.tag == 'code');
                            if (codes.length == 0) {
                                swal("商品コードを選択してください", "", "error");
                                return false;
                            }
                            if (codes.length > 1) {
                                swal("商品コードは1つだけ選択してください", "", "error");
                                return false;
                            }

                            var dates = naite.vuemodel.$data.fields.filter(a => a.tag == 'date');
                            if (dates.length == 0) {
                                swal("日付を選択してください", "", "error");
                                return false;
                            }
                            if (dates.length > 1) {
                                swal("日付は1つだけ選択してください", "", "error");
                                return false;
                            }

                            var quantities = naite.vuemodel.$data.fields.filter(a => a.tag == 'quantity');
                            if (quantities.length == 0) {
                                swal("数量を選択してください", "", "error");
                                return false;
                            }
                            if (quantities.length > 1) {
                                swal("数量は1つだけ選択してください", "", "error");
                                return false;
                            }

                            swal({
                                title: "登録しますか？",
                                text: "",
                                type: "warning",
                                showCancelButton: true,
                                confirmButtonClass: "btn btn-teal",
                                confirmButtonText: "はい",
                                cancelButtonClass: "btn btn-default",
                                cancelButtonText: "キャンセル"
                            }).then(function (isConfirm) {
                                if (isConfirm.value) {
                                    // 二重クリック防止
                                    naite.loading(true);
                                    naite.vuemodel.putData();

                                } else {
                                    swal.closeModal();
                                }
                            });
                        }
                    });
            }
        }
    });
});