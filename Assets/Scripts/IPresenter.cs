public interface IPresenter<T> where T : IGameState {
	void Present(T gameState);
}
