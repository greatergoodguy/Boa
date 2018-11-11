public interface IPresenter<T> {
	void Present(T gameState);
	void Clean();
}
