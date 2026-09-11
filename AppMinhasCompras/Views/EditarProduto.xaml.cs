using AppMinhasCompras.Models;

namespace AppMinhasCompras.Views;

public partial class EditarProduto : ContentPage
{
    public EditarProduto()
    {
        InitializeComponent();
    }

    private async void ToolbarItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            Produto produto_anexado = BindingContext as Produto;

            string categoriaFinal = "";

            if (!string.IsNullOrWhiteSpace(txt_nova_categoria.Text))
            {
                categoriaFinal = txt_nova_categoria.Text.Trim();
            }
            else if (pck_categoria_cadastro.SelectedItem != null)
            {
                categoriaFinal = pck_categoria_cadastro.SelectedItem.ToString();
            }

            if (!string.IsNullOrEmpty(categoriaFinal))
            {
                categoriaFinal = char.ToUpper(categoriaFinal[0]) + categoriaFinal.Substring(1).ToLower();
            }

            Produto p = new Produto

            {
                Id = produto_anexado.Id,
                Descricao = txt_descricao.Text,
                Quantidade = Convert.ToDouble(txt_quantidade.Text),
                Categoria = categoriaFinal,
                Preco = Convert.ToDouble(txt_preco.Text)
            };

            await App.Db.Update(p);
            await DisplayAlert("Sucesso!", "Registro Atualizado", "OK");
            await Navigation.PopAsync();

        }
        catch (Exception ex)
        {
            DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void CargarCategoriasExistentes()
    {
        Produto produto_anexado = BindingContext as Produto;

        List<Produto> todos = await App.Db.GetAll();

        var categorias = todos
            .Where(p => !string.IsNullOrEmpty(p.Categoria))
            .Select(p => char.ToUpper(p.Categoria[0]) + p.Categoria.Substring(1).ToLower())
            .Distinct()
            .ToList();

        pck_categoria_cadastro.ItemsSource = categorias;

        if (produto_anexado != null && !string.IsNullOrEmpty(produto_anexado.Categoria))
        {
            string catFormatada = char.ToUpper(produto_anexado.Categoria[0]) + produto_anexado.Categoria.Substring(1).ToLower();
            pck_categoria_cadastro.SelectedItem = catFormatada;
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CargarCategoriasExistentes();
    }
}