using AppMinhasCompras.Models;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace AppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{
	ObservableCollection<Produto> lista = new ObservableCollection<Produto>();

	public ListaProduto()
	{
		InitializeComponent();

		lst_produtos.ItemsSource = lista;
    }

    protected async override void OnAppearing() 
	{
		try 
        {
            lista.Clear();

            List<Produto> tmp = await App.Db.GetAll();

		    tmp.ForEach(i => lista.Add(i));

            PreencherPikerCategorias();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
       
    }

    private async void PreencherPikerCategorias()
    {
        List<Produto> lista_produtos = await App.Db.GetAll();

        var categorias = lista_produtos
            .Where(p => !string.IsNullOrEmpty(p.Categoria))
            .Select(p => p.Categoria)
            .Distinct()  
            .ToList();

        categorias.Insert(0, "Todas");

        pck_categoria.ItemsSource = categorias;
    }

    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
		try
		{
			Navigation.PushAsync(new Views.NovoProduto());

		} catch (Exception ex)
		{
			DisplayAlert("Ops", ex.Message, "OK");
		}
    }

    private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
        try 
        { 
            string q = e.NewTextValue;

            lst_produtos.IsRefreshing = true;

            lista.Clear();

            List<Produto> tmp = await App.Db.Search(q);

            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
        finally
        {
            lst_produtos.IsRefreshing = false;
        }
    }

    private async void ToolbarItem_Clicked_1(object sender, EventArgs e)
    {
        try
        {
            List<Produto> todosProdutos = await App.Db.GetAll();

            if (todosProdutos == null || todosProdutos.Count == 0)
            {
                await DisplayAlert("Relatório", "Nenhum produto cadastrado para calcular.", "OK");
                return;
            }

            Dictionary<string, double> relatorioCategoria = new Dictionary<string, double>();

            foreach (Produto p in todosProdutos)
            {
                string cat = string.IsNullOrWhiteSpace(p.Categoria) ? "Sem Categoria" : p.Categoria;

                if (relatorioCategoria.ContainsKey(cat))
                {
                    relatorioCategoria[cat] += p.Total;
                }
                else
                {
                    relatorioCategoria[cat] = p.Total;
                }
            }

            string mensagem = "Resumo de Gastos por Categoria:\n\n";

            foreach (var item in relatorioCategoria)
            {
                mensagem += $"• {item.Key}: {item.Value:C}\n";
            }

            double totalGeral = todosProdutos.Sum(p => p.Total);
            mensagem += $"\n-------------------\nTotal Geral: {totalGeral:C}";

            await DisplayAlert("Relatório de Gastos", mensagem, "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void MenuItem_Clicked(object sender, EventArgs e)
    {
        try 
        {
            MenuItem selecionado = sender as MenuItem;

            Produto p = selecionado.BindingContext as Produto;

            bool confirm = await DisplayAlert("Tem Certeza?", $"Remover {p.Descricao}?", "Sim", "Não");

            if (confirm) 
            {
                await App.Db.Delete(p.Id);
                lista.Remove(p);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try 
        {
            Produto p = e.SelectedItem as Produto;

            Navigation.PushAsync(new Views.EditarProduto
            {
                BindingContext = p,
            });
        }
        catch (Exception ex) 
        {
            DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void lst_produtos_Refreshing(object sender, EventArgs e)
    {
        try
        {
            lista.Clear();

            List<Produto> tmp = await App.Db.GetAll();

            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
        finally
        {
            lst_produtos.IsRefreshing = false;
        }
    }

    private async void pck_categoria_SelectedIndexChanged(object sender, EventArgs e)
    {
        try
        {
            string categoriaSelecionada = pck_categoria.SelectedItem as string;

            lista.Clear();

            List<Produto> tmp;

            if (string.IsNullOrEmpty(categoriaSelecionada) || categoriaSelecionada == "Todas")
            {
                tmp = await App.Db.GetAll();
            }
            else
            {
                tmp = await App.Db.GetByCategoria(categoriaSelecionada);
            }

            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        }
    }
}